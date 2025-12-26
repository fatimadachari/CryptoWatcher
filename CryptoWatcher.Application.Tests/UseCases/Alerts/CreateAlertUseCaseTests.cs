using CryptoWatcher.Application.DTOs.Requests;
using CryptoWatcher.Application.Interfaces.Repositories;
using CryptoWatcher.Application.UseCases.Alerts;
using CryptoWatcher.Domain.Entities;
using CryptoWatcher.Domain.Enums;
using FluentAssertions;
using Moq;

namespace CryptoWatcher.Application.Tests.UseCases.Alerts;

public class CreateAlertUseCaseTests
{
    private readonly Mock<IAlertRepository> _mockAlertRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateAlertUseCase _useCase;

    public CreateAlertUseCaseTests()
    {
        _mockAlertRepository = new Mock<IAlertRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _useCase = new CreateAlertUseCase(_mockAlertRepository.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateAlert()
    {
        // Arrange
        var request = new CreateAlertRequest(
            UserId: 1,
            CryptoSymbol: "BTC",
            TargetPrice: 50000,
            Condition: PriceCondition.Below
        );

        _mockUserRepository
            .Setup(r => r.ExistsAsync(request.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockAlertRepository
            .Setup(r => r.CreateAsync(It.IsAny<CryptoAlert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CryptoAlert alert, CancellationToken _) =>
            {
                typeof(CryptoAlert).GetProperty("Id")!.SetValue(alert, 1);
                return alert;
            });

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.UserId.Should().Be(request.UserId);
        result.CryptoSymbol.Should().Be(request.CryptoSymbol);
        result.TargetPrice.Should().Be(request.TargetPrice);
        result.Condition.Should().Be(request.Condition);
        result.Status.Should().Be(AlertStatus.Active);

        _mockUserRepository.Verify(
            r => r.ExistsAsync(request.UserId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _mockAlertRepository.Verify(
            r => r.CreateAsync(It.IsAny<CryptoAlert>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateAlertRequest(
            UserId: 999,
            CryptoSymbol: "BTC",
            TargetPrice: 50000,
            Condition: PriceCondition.Below
        );

        _mockUserRepository
            .Setup(r => r.ExistsAsync(request.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*Usuário {request.UserId}*não encontrado*");

        _mockAlertRepository.Verify(
            r => r.CreateAsync(It.IsAny<CryptoAlert>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidSymbol_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateAlertRequest(
            UserId: 1,
            CryptoSymbol: "",
            TargetPrice: 50000,
            Condition: PriceCondition.Below
        );

        _mockUserRepository
            .Setup(r => r.ExistsAsync(request.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Símbolo*");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPrice_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateAlertRequest(
            UserId: 1,
            CryptoSymbol: "BTC",
            TargetPrice: -100,
            Condition: PriceCondition.Below
        );

        _mockUserRepository
            .Setup(r => r.ExistsAsync(request.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Preço alvo deve ser maior que zero*");
    }
}