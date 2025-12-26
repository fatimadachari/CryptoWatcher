using CryptoWatcher.Application.DTOs.Requests;
using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Application.UseCases.Users;
using CryptoWatcher.Domain.Entities;
using FluentAssertions;
using Moq;

namespace CryptoWatcher.Application.Tests.UseCases.Users;

public class CreateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateUserUseCase _useCase;

    public CreateUserUseCaseTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _useCase = new CreateUserUseCase(_mockUserRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var request = new CreateUserRequest("test@example.com", "Test User");

        _mockUserRepository
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null); // Email não existe

        _mockUserRepository
            .Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) =>
            {
                // Simula o banco gerando um ID
                typeof(User).GetProperty("Id")!.SetValue(user, 1);
                return user;
            });

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Email.Should().Be(request.Email);
        result.Name.Should().Be(request.Name);

        _mockUserRepository.Verify(
            r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _mockUserRepository.Verify(
            r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateUserRequest("existing@example.com", "Test User");
        var existingUser = new User("existing@example.com", "Existing User");

        _mockUserRepository
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser); // Email já existe

        // Act
        var act = async () => await _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*{request.Email}*já existe*");

        _mockUserRepository.Verify(
            r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never // Não deve tentar criar
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateUserRequest("invalid-email", "Test User");

        _mockUserRepository
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null); // Email não existe (mas é inválido)

        // Act
        var act = async () => await _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Email inválido*");

        // O repositório deve ser consultado, mas não deve criar
        _mockUserRepository.Verify(
            r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()),
            Times.Once // É consultado primeiro
        );

        _mockUserRepository.Verify(
            r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never // Mas não deve criar
        );
    }
}