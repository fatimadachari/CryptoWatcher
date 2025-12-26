using System.Net;
using System.Net.Http.Json;
using CryptoWatcher.Application.DTOs.Requests;
using CryptoWatcher.Application.DTOs.Responses;
using FluentAssertions;

namespace CryptoWatcher.API.IntegrationTests.Controllers;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateUserRequest("integration@test.com", "Integration Test User");

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
        user!.Id.Should().BeGreaterThan(0);
        user.Email.Should().Be(request.Email);
        user.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateUserRequest("duplicate@test.com", "Duplicate User");

        // Criar primeiro usuário
        await _client.PostAsJsonAsync("/api/users", request);

        // Act - Tentar criar novamente com mesmo email
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        error.Should().ContainKey("error");
        error!["error"].Should().Contain("já existe");
    }

    [Fact]
    public async Task CreateUser_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateUserRequest("invalid-email", "Test User");

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateUserRequest("test@test.com", "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}