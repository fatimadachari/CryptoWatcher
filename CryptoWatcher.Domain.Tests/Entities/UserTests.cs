using CryptoWatcher.Domain.Entities;
using CryptoWatcher.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace CryptoWatcher.Domain.Tests.Entities;

public class UserTests
{
    private const string ValidPasswordHash = "$2a$11$abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJ";

    [Fact]
    public void Constructor_WithValidData_ShouldCreateUser()
    {
        // Arrange & Act
        var user = new User("test@example.com", "Test User", ValidPasswordHash);

        // Assert
        user.Email.Should().Be("test@example.com");
        user.Name.Should().Be("Test User");
        user.PasswordHash.Should().Be(ValidPasswordHash);
        user.Alerts.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-email")]
    [InlineData("missing@")]
    public void Constructor_WithInvalidEmail_ShouldThrowArgumentException(string? invalidEmail)
    {
        // Arrange & Act & Assert
        var act = () => new User(invalidEmail!, "Test User", ValidPasswordHash);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Email*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("AB")]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Arrange & Act & Assert
        var act = () => new User("test@example.com", invalidName!, ValidPasswordHash);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Nome*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidPasswordHash_ShouldThrowArgumentException(string? invalidHash)
    {
        // Arrange & Act & Assert
        var act = () => new User("test@example.com", "Test User", invalidHash!);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*senha*");
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var user = new User("test@example.com", "Old Name", ValidPasswordHash);
        var oldUpdatedAt = user.UpdatedAt;

        // Act
        user.UpdateName("New Name");

        // Assert
        user.Name.Should().Be("New Name");
        user.UpdatedAt.Should().BeAfter(oldUpdatedAt!.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("AB")]
    public void UpdateName_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Arrange
        var user = new User("test@example.com", "Test User", ValidPasswordHash);

        // Act & Assert
        var act = () => user.UpdateName(invalidName!);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Nome*");
    }

    [Fact]
    public void AddAlert_ShouldAddAlertToCollection()
    {
        // Arrange
        var user = new User("test@example.com", "Test User", ValidPasswordHash);
        var alert = new CryptoAlert(user.Id, "BTC", 50000, PriceCondition.Above);

        // Act
        user.AddAlert(alert);

        // Assert
        user.Alerts.Should().ContainSingle()
            .Which.Should().Be(alert);
    }
}