using FluentAssertions;
using JwtAuthApi.Models;
using JwtAuthApi.Services;

namespace WeatherAPI.Tests;

public class UserServiceTests
{
    private readonly InMemoryUserService _service;

    public UserServiceTests()
    {
        _service = new InMemoryUserService();
    }

    [Fact]
    public async Task CreateAsync_ValidUser_ReturnsUserWithId()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };

        // Act
        var result = await _service.CreateAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task CreateAsync_MultipleUsers_IncrementsId()
    {
        // Arrange
        var user1 = new User { Username = "user1", Email = "user1@example.com", PasswordHash = "hash1" };
        var user2 = new User { Username = "user2", Email = "user2@example.com", PasswordHash = "hash2" };

        // Act
        var result1 = await _service.CreateAsync(user1);
        var result2 = await _service.CreateAsync(user2);

        // Assert
        result1.Id.Should().Be(1);
        result2.Id.Should().Be(2);
    }

    [Fact]
    public async Task GetByUsernameAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };
        await _service.CreateAsync(user);

        // Act
        var result = await _service.GetByUsernameAsync("testuser");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByUsernameAsync_NonExistingUser_ReturnsNull()
    {
        // Act
        var result = await _service.GetByUsernameAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUsernameAsync_CaseSensitive_ReturnsCorrectUser()
    {
        // Arrange
        var user = new User
        {
            Username = "TestUser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };
        await _service.CreateAsync(user);

        // Act
        var result = await _service.GetByUsernameAsync("testuser");

        // Assert
        result.Should().BeNull();
    }
}
