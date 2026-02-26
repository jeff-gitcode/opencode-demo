using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using JwtAuthApi.Controllers;
using JwtAuthApi.DTOs;
using JwtAuthApi.Models;
using JwtAuthApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Moq;

namespace WeatherAPI.Tests;

public class AuthControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly IConfiguration _configuration;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        
        // Create test configuration
        var inMemorySettings = new Dictionary<string, string?> {
            {"Jwt:Key", "ThisIsATestKeyThatIsAtLeast32CharactersLong!"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"}
        };
        
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
            
        _controller = new AuthController(_mockUserService.Object, _configuration);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new RegisterRequest 
        { 
            Username = "testuser", 
            Email = "test@example.com", 
            Password = "password123" 
        };
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync("testuser"))
            .ReturnsAsync((User?)null);
            
        _mockUserService
            .Setup(s => s.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_ExistingUsername_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest 
        { 
            Username = "existinguser", 
            Email = "test@example.com", 
            Password = "password123" 
        };
        
        var existingUser = new User 
        { 
            Id = 1, 
            Username = "existinguser",
            Email = "existing@example.com",
            PasswordHash = "hash"
        };
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync("existinguser"))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var request = new LoginRequest 
        { 
            Username = "testuser", 
            Password = "password123" 
        };
        
        var user = new User 
        { 
            Id = 1, 
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync("testuser"))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var authResponse = okResult!.Value as AuthResponse;
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.Username.Should().Be("testuser");
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
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync("nonexistent"))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest 
        { 
            Username = "testuser", 
            Password = "wrongpassword" 
        };
        
        var user = new User 
        { 
            Id = 1, 
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync("testuser"))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}
