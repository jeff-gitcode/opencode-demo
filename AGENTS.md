# AGENTS.md - Developer Guidelines for This Project

## Project Overview

This is a .NET 8.0 ASP.NET Core Web API project with JWT authentication. The application provides user registration and login endpoints with token-based authentication.

## Project Architecture Diagram


---

## Technology Stack

- **Framework**: .NET 8.0 (ASP.NET Core Web API)
- **Authentication**: JWT (JSON Web Tokens) with Bearer scheme
- **Database**: In-memory (Microsoft.EntityFrameworkCore.InMemory)
- **Password Hashing**: BCrypt.Net-Next
- **API Documentation**: Swashbuckle (Swagger/OpenAPI)
- **Language**: C# 10+ with nullable reference types enabled

---

## Build, Run, and Test Commands

### Build the Project

```bash
# Build the entire solution
dotnet build

# Build specific project
dotnet build WeatherAPI/JwtAuthApi.csproj
```

### Run the Application

```bash
# Run in development mode (from project directory)
dotnet run --project WeatherAPI/JwtAuthApi.csproj

# Or run from solution root with environment
dotnet run --project WeatherAPI/JwtAuthApi.csproj --environment Development
```

The API runs on `http://localhost:5000` by default. Swagger UI is available at `/swagger` in Development mode.

### Testing

**No unit tests currently exist in this project.** To add tests:

```bash
# Create a test project
dotnet new xunit -n WeatherAPI.Tests

# Add reference to main project
dotnet add WeatherAPI.Tests reference WeatherAPI/JwtAuthApi.csproj

# Run all tests
dotnet test

# Run a single test (by test name)
dotnet test --filter "FullyQualifiedName~TestMethodName"

# Run tests in a specific project
dotnet test WeatherAPI.Tests/WeatherAPI.Tests.csproj
```

### Linting and Code Analysis

This project does not have a linter configured. For .NET projects, consider adding:

- **DotNetAnalyzers** for Roslyn analyzers
- **StyleCopAnalyzers** for style enforcement
- **SonarAnalyzer.CSharp** for quality gates

```bash
# Run static analysis
dotnet build  # Includes Roslyn analyzers if installed

# Clean build artifacts
dotnet clean
```

---

## Code Style Guidelines

### General Conventions

- **.NET version**: Target .NET 8.0
- **Language version**: C# 10+ (implicit usings enabled, nullable reference types enabled)
- **Framework**: ASP.NET Core Web API

### File Organization

```
WeatherAPI/
├── Controllers/      # API controllers (suffix: Controller)
├── Services/        # Business logic services
├── Models/          # Domain entities
├── DTOs/            # Data Transfer Objects
└── Program.cs       # Application entry point
```

### Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Classes | PascalCase | `AuthController`, `UserService` |
| Interfaces | PascalCase with `I` prefix | `IUserService` |
| Methods | PascalCase | `GetByUsernameAsync`, `CreateAsync` |
| Properties | PascalCase | `Username`, `PasswordHash` |
| Local variables | camelCase | `existingUser`, `jwtKey` |
| Parameters | camelCase | `request.Username` |
| Namespaces | PascalCase | `JwtAuthApi.Controllers` |
| Files | Match class name | `AuthController.cs` |

### Namespace Style

Use **file-scoped namespaces** (C# 10+):

```csharp
namespace JwtAuthApi.Controllers;
```

### Import Organization

Order imports as follows (dotnet format default):

1. System namespaces (`System.*`)
2. Microsoft namespaces (`Microsoft.*`)
3. Third-party namespaces
4. Project namespaces

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtAuthApi.DTOs;
using JwtAuthApi.Models;
using JwtAuthApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
```

### Type Definitions

- **Records**: Use `record` for immutable DTOs
- **Classes**: Use `class` for mutable entities and services
- **Interfaces**: Define contracts for services

```csharp
// DTO (immutable)
public record LoginRequest
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

// Entity (mutable)
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
}

// Interface
public interface IUserService
{
    Task<User?> GetByUsernameAsync(string username);
}
```

### Async/Await

- Use `async`/`await` for I/O-bound operations
- Use `Task` return types for async methods
- Always include `Async` suffix for async method names

```csharp
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    var user = await _userService.GetByUsernameAsync(request.Username);
    // ...
}
```

### Error Handling

- Use `[ApiController]` for automatic model validation
- Return appropriate HTTP status codes:
  - `200 OK` for successful GET/POST
  - `201 Created` for resource creation
  - `400 BadRequest` for validation errors
  - `401 Unauthorized` for authentication failures
  - `404 NotFound` for missing resources
- Use anonymous objects for error responses

```csharp
if (existingUser != null)
    return BadRequest(new { message = "Username already exists" });

if (user == null)
    return Unauthorized(new { message = "Invalid credentials" });
```

### Dependency Injection

Register services in `Program.cs`:

```csharp
builder.Services.AddSingleton<IUserService, InMemoryUserService>();
```

Inject via constructor:

```csharp
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }
}
```

### Controller Guidelines

- Inherit from `ControllerBase` for API controllers
- Use `[ApiController]` attribute
- Use route attributes: `[Route("api/[controller]")]`
- Return `IActionResult` or specific action result types

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // ...
}
```

### Configuration

- Store configuration in `appsettings.json` and `appsettings.Development.json`
- Use `IConfiguration` to access settings
- Use null-forgiving operator (`!`) when configuration is guaranteed in non-development

```csharp
var jwtKey = builder.Configuration["Jwt:Key"]!;
```

---

## Testing Guidelines

When adding tests:

1. **Use xUnit** as the testing framework (standard for .NET)
2. **Use Moq** for mocking dependencies
3. **Use FluentAssertions** for readable assertions

```bash
# Add testing packages
dotnet add package xunit
dotnet add package Moq
dotnet add package FluentAssertions
```

### Example Test Structure

```csharp
public class AuthControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        var config = new ConfigurationBuilder().Build();
        _controller = new AuthController(_mockUserService.Object, config);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var request = new LoginRequest { Username = "test", Password = "password" };
        _mockUserService.Setup(s => s.GetByUsernameAsync("test"))
            .ReturnsAsync(new User { Id = 1, Username = "test" });

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
```

---

## Common Tasks

### Adding a New Controller

1. Create file in `Controllers/` directory
2. Name file with `Controller` suffix
3. Inherit from `ControllerBase`
4. Add `[ApiController]` and `[Route]` attributes

### Adding a New Service

1. Define interface in `Services/` directory
2. Implement interface as concrete class
3. Register in `Program.cs` with DI container

### Adding a New DTO

1. Create file in `DTOs/` directory
2. Use `record` for immutable DTOs
3. Use `class` if mutable properties needed

---

## Git Conventions

- Use meaningful commit messages
- Create feature branches for new features
- Run `dotnet build` before committing to catch compilation errors

---

## Notes for AI Agents

- Always verify changes compile with `dotnet build` before reporting completion
- This project uses in-memory storage - data is lost on restart
- JWT tokens expire after 1 hour (configurable in token generation)
- Always use parameter binding attributes (`[FromBody]`, `[FromRoute]`) explicitly
- Never commit secrets or keys to version control
