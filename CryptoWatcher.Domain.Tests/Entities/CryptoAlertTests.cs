using CryptoWatcher.Domain.Entities;
using CryptoWatcher.Domain.Enums;
using FluentAssertions;

namespace CryptoWatcher.Domain.Tests.Entities;

public class CryptoAlertTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateAlert()
    {
        // Arrange
        var userId = 1;
        var symbol = "BTC";
        var targetPrice = 50000m;
        var condition = PriceCondition.Below;

        // Act
        var alert = new CryptoAlert(userId, symbol, targetPrice, condition);

        // Assert
        alert.UserId.Should().Be(userId);
        alert.CryptoSymbol.Should().Be(symbol.ToUpper());
        alert.TargetPrice.Should().Be(targetPrice);
        alert.Condition.Should().Be(condition);
        alert.Status.Should().Be(AlertStatus.Active);
        alert.TriggeredAt.Should().BeNull();
    }

    [Theory]
    [InlineData("btc", "BTC")]
    [InlineData("eth", "ETH")]
    [InlineData("DoGe", "DOGE")]
    public void Constructor_ShouldConvertSymbolToUpperCase(string input, string expected)
    {
        // Arrange & Act
        var alert = new CryptoAlert(1, input, 50000, PriceCondition.Below);

        // Assert
        alert.CryptoSymbol.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidSymbol_ShouldThrowArgumentException(string invalidSymbol)
    {
        // Arrange & Act
        var act = () => new CryptoAlert(1, invalidSymbol, 50000, PriceCondition.Below);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Símbolo*");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("VERYLONGSYMBOL")]
    public void Constructor_WithInvalidSymbolLength_ShouldThrowArgumentException(string invalidSymbol)
    {
        // Arrange & Act
        var act = () => new CryptoAlert(1, invalidSymbol, 50000, PriceCondition.Below);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*entre 2 e 10 caracteres*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Constructor_WithInvalidPrice_ShouldThrowArgumentException(decimal invalidPrice)
    {
        // Arrange & Act
        var act = () => new CryptoAlert(1, "BTC", invalidPrice, PriceCondition.Below);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Preço alvo deve ser maior que zero*");
    }

    [Fact]
    public void ShouldTrigger_WhenPriceIsBelowTarget_AndConditionIsBelow_ShouldReturnTrue()
    {
        // Arrange
        var alert = new CryptoAlert(1, "BTC", 50000, PriceCondition.Below);
        var currentPrice = 49000m;

        // Act
        var result = alert.ShouldTrigger(currentPrice);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldTrigger_WhenPriceIsAboveTarget_AndConditionIsBelow_ShouldReturnFalse()
    {
        // Arrange
        var alert = new CryptoAlert(1, "BTC", 50000, PriceCondition.Below);
        var currentPrice = 51000m;

        // Act
        var result = alert.ShouldTrigger(currentPrice);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ShouldTrigger_WhenPriceIsAboveTarget_AndConditionIsAbove_ShouldReturnTrue()
    {
        // Arrange
        var alert = new CryptoAlert(1, "BTC", 50000, PriceCondition.Above);
        var currentPrice = 51000m;

        // Act
        var result = alert.ShouldTrigger(currentPrice);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldTrigger_WhenPriceIsBelowTarget_AndConditionIsAbove_ShouldReturnFalse()
    {
        // Arrange
        var alert = new CryptoAlert(1, "BTC", 50000, PriceCondition.Above);
        var currentPrice = 49000m;

        // Act
        var result = alert.ShouldTrigger(currentPrice);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ShouldTrigger_WhenAlertIsNotActive_ShouldReturnFalse()
    {
        // Arrange
        var alert = new CryptoAlert(1, "BTC", 50000, PriceCondition.Below);
        alert.MarkAsTriggered(); // Muda status para Triggered
        var currentPrice = 49000m;

        // Act
        var result = alert.ShouldTrigger(currentPrice);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void MarkAsTriggered_WhenAlertIsActive_ShouldChangeStatusToTriggered()
    {
        // Arrange
        var alert = new CryptoAlert(1, "BTC", 50000, PriceCondition.Below);

        // Act
        alert.MarkAsTriggered();

        // Assert
        alert.Status.Should().Be(AlertStatus.Triggered);
        alert.TriggeredAt.Should().NotBeNull();
        alert.TriggeredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        alert.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsTriggered_WhenAlertIsNotActive_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var alert = new CryptoAlert(1, "BTC", 50000, PriceCondition.Below);
        alert.MarkAsTriggered(); // Já disparado

        // Act
        var act = () => alert.MarkAsTriggered();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Apenas alertas ativos podem ser disparados*");
    }

    [Fact]
    public void Cancel_WhenAlertIsActive_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var alert = new CryptoAlert(1, "BTC", 50000, PriceCondition.Below);

        // Act
        alert.Cancel();

        // Assert
        alert.Status.Should().Be(AlertStatus.Cancelled);
        alert.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_WhenAlertIsTriggered_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var alert = new CryptoAlert(1, "BTC", 50000, PriceCondition.Below);
        alert.MarkAsTriggered();

        // Act
        var act = () => alert.Cancel();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Não é possível cancelar um alerta já disparado*");
    }
}