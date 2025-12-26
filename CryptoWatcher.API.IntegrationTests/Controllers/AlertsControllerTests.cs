using System.Net;
using System.Net.Http.Json;
using CryptoWatcher.Application.DTOs.Requests;
using CryptoWatcher.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace CryptoWatcher.API.IntegrationTests.Controllers;

public class AlertsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AlertsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAlert_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new CreateAlertRequest(
            CryptoSymbol: "BTC",
            TargetPrice: 50000,
            Condition: PriceCondition.Above
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/alerts", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetActiveAlerts_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/alerts/active");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}