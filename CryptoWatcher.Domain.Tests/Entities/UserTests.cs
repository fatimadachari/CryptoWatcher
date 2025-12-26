using CryptoWatcher.Domain.Entities;
using FluentAssertions;

namespace CryptoWatcher.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var email = "test@example.com";
        var name = "Test User";

        // Act
        var user = new User(email, name);

        // Assert
        user.Email.Should().Be(email);
        user.Name.Should().Be(name);
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.Alerts.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidEmail_ShouldThrowArgumentException(string invalidEmail)
    {
        // Arrange & Act
        var act = () => new User(invalidEmail, "Test User");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email*");
    }

    [Fact]
    public void Constructor_WithEmailWithoutAt_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new User("invalidemail.com", "Test User");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email inválido*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange & Act
        var act = () => new User("test@example.com", invalidName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Nome*");
    }

    [Fact]
    public void Constructor_WithShortName_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new User("test@example.com", "Ab");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Nome deve ter no mínimo 3 caracteres*");
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var user = new User("test@example.com", "Original Name");
        var newName = "Updated Name";

        // Act
        user.UpdateName(newName);

        // Assert
        user.Name.Should().Be(newName);
        user.UpdatedAt.Should().NotBeNull();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateName_WithInvalidName_ShouldThrowArgumentException()
    {
        // Arrange
        var user = new User("test@example.com", "Original Name");

        // Act
        var act = () => user.UpdateName("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddAlert_ShouldAddAlertToCollection()
    {
        // Arrange
        var user = new User("test@example.com", "Test User");
        var alert = new CryptoAlert(user.Id, "BTC", 50000, Domain.Enums.PriceCondition.Below);

        // Act
        user.AddAlert(alert);

        // Assert
        user.Alerts.Should().HaveCount(1);
        user.Alerts.Should().Contain(alert);
    }
}