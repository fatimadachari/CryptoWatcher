using System.Net;
using System.Net.Http.Json;
using CryptoWatcher.Application.DTOs.Requests;
using CryptoWatcher.Application.DTOs.Responses;
using CryptoWatcher.Domain.Enums;
using FluentAssertions;

namespace CryptoWatcher.API.IntegrationTests.Controllers;

public class AlertsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AlertsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAlert_WithValidData_ShouldReturnCreated()
    {
        // Arrange - Criar usuário primeiro
        var userRequest = new CreateUserRequest("alert@test.com", "Alert Test User");
        var userResponse = await _client.PostAsJsonAsync("/api/users", userRequest);
        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();

        var alertRequest = new CreateAlertRequest(
            UserId: user!.Id,
            CryptoSymbol: "BTC",
            TargetPrice: 50000,
            Condition: PriceCondition.Below
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/alerts", alertRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var alert = await response.Content.ReadFromJsonAsync<AlertResponse>();
        alert.Should().NotBeNull();
        alert!.Id.Should().BeGreaterThan(0);
        alert.UserId.Should().Be(user.Id);
        alert.CryptoSymbol.Should().Be("BTC");
        alert.TargetPrice.Should().Be(50000);
        alert.Condition.Should().Be(PriceCondition.Below);
        alert.Status.Should().Be(AlertStatus.Active);
    }

    [Fact]
    public async Task CreateAlert_WithNonExistentUser_ShouldReturnBadRequest()
    {
        // Arrange
        var alertRequest = new CreateAlertRequest(
            UserId: 99999,
            CryptoSymbol: "BTC",
            TargetPrice: 50000,
            Condition: PriceCondition.Below
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/alerts", alertRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetActiveAlerts_ShouldReturnAllActiveAlerts()
    {
        // Arrange - Criar usuário e alertas
        var userRequest = new CreateUserRequest("active@test.com", "Active Test User");
        var userResponse = await _client.PostAsJsonAsync("/api/users", userRequest);
        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();

        // Criar 2 alertas
        await _client.PostAsJsonAsync("/api/alerts", new CreateAlertRequest(
            user!.Id, "BTC", 50000, PriceCondition.Below
        ));
        await _client.PostAsJsonAsync("/api/alerts", new CreateAlertRequest(
            user.Id, "ETH", 3000, PriceCondition.Above
        ));

        // Act
        var response = await _client.GetAsync("/api/alerts/active");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var alerts = await response.Content.ReadFromJsonAsync<List<AlertResponse>>();
        alerts.Should().NotBeNull();
        alerts!.Should().HaveCountGreaterOrEqualTo(2);
        alerts.Should().OnlyContain(a => a.Status == AlertStatus.Active);
    }

    [Fact]
    public async Task CreateAlert_WithInvalidSymbol_ShouldReturnBadRequest()
    {
        // Arrange - Criar usuário
        var userRequest = new CreateUserRequest("invalid@test.com", "Invalid Test User");
        var userResponse = await _client.PostAsJsonAsync("/api/users", userRequest);
        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();

        var alertRequest = new CreateAlertRequest(
            UserId: user!.Id,
            CryptoSymbol: "", // Inválido
            TargetPrice: 50000,
            Condition: PriceCondition.Below
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/alerts", alertRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}