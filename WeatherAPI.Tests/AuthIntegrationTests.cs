using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JwtAuthApi.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using JwtAuthApi.Services;

namespace WeatherAPI.Tests;

public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Ensure we use a fresh InMemoryUserService for each test
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUserService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddSingleton<IUserService, InMemoryUserService>();
            });
        });
        
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = $"testuser_{Guid.NewGuid():N}",
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("registered successfully");
    }

    [Fact]
    public async Task Register_DuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var username = $"duplicate_{Guid.NewGuid():N}";
        var request = new RegisterRequest
        {
            Username = username,
            Email = "test@example.com",
            Password = "password123"
        };

        // First registration should succeed
        await _client.PostAsJsonAsync("/api/auth/register", request);

        // Act
        // Second registration with same username should fail
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var username = $"logintest_{Guid.NewGuid():N}";
        var password = "password123";
        
        var registerRequest = new RegisterRequest
        {
            Username = username,
            Email = "test@example.com",
            Password = password
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Username = username,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.Username.Should().Be(username);
    }

    [Fact]
    public async Task Login_InvalidUsername_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "nonexistent",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var username = $"loginfail_{Guid.NewGuid():N}";
        var password = "correctpassword";
        
        var registerRequest = new RegisterRequest
        {
            Username = username,
            Email = "test@example.com",
            Password = password
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Username = username,
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
