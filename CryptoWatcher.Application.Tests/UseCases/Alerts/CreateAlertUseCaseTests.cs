using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Application.UseCases.Alerts;
using CryptoWatcher.Domain.Entities;
using CryptoWatcher.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace CryptoWatcher.Application.Tests.UseCases.Alerts;

public class CreateAlertUseCaseTests
{
    private readonly Mock<IAlertRepository> _mockAlertRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly CreateAlertUseCase _useCase;

    public CreateAlertUseCaseTests()
    {
        _mockAlertRepo = new Mock<IAlertRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _useCase = new CreateAlertUseCase(_mockAlertRepo.Object, _mockUserRepo.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateAlert()
    {
        // Arrange
        var userId = 1;
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("test123");
        var user = new User("test@test.com", "Test User", passwordHash);

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId, default))
            .ReturnsAsync(user);

        // Act
        var result = await _useCase.ExecuteAsync(
            userId,
            "BTC",
            50000,
            PriceCondition.Above
        );

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.CryptoSymbol.Should().Be("BTC");
        result.TargetPrice.Should().Be(50000);
        result.Condition.Should().Be(PriceCondition.Above);
        _mockAlertRepo.Verify(r => r.CreateAsync(It.IsAny<CryptoAlert>(), default), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = 999;

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId, default))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _useCase.ExecuteAsync(
            userId,
            "BTC",
            50000,
            PriceCondition.Above
        );

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*não encontrado*");
        _mockAlertRepo.Verify(r => r.CreateAsync(It.IsAny<CryptoAlert>(), default), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidSymbol_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = 1;
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("test123");
        var user = new User("test@test.com", "Test User", passwordHash);

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId, default))
            .ReturnsAsync(user);

        // Act
        var act = async () => await _useCase.ExecuteAsync(
            userId,
            "", // símbolo vazio
            50000,
            PriceCondition.Above
        );

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}