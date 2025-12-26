using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Application.UseCases.Users;
using CryptoWatcher.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CryptoWatcher.Application.Tests.UseCases.Users;

public class CreateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly CreateUserUseCase _useCase;

    public CreateUserUseCaseTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _useCase = new CreateUserUseCase(_mockRepo.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var email = "test@test.com";
        var name = "Test User";

        _mockRepo.Setup(r => r.GetByEmailAsync(email, default))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _useCase.ExecuteAsync(email, name);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
        result.Name.Should().Be(name);
        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<User>(), default), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var email = "existing@test.com";
        var name = "Test User";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("test123");
        var existingUser = new User(email, "Existing", passwordHash);

        _mockRepo.Setup(r => r.GetByEmailAsync(email, default))
            .ReturnsAsync(existingUser);

        // Act
        var act = async () => await _useCase.ExecuteAsync(email, name);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*já está cadastrado*");
        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<User>(), default), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidEmail = "invalid-email";
        var name = "Test User";

        _mockRepo.Setup(r => r.GetByEmailAsync(invalidEmail, default))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _useCase.ExecuteAsync(invalidEmail, name);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}