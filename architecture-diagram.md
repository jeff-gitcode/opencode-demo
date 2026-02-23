```
+------------------------------------------------------------------+
|                        HTTP Requests                             |
|              (http://localhost:5000/api/*)                      |
+----------------------------+-------------------------------------+
                             |
                             v
+------------------------------------------------------------------+
|                      ASP.NET Core Pipeline                       |
|  +---------------+  +----------------+  +--------------------+  |
|  |   SwaggerUI   |  | Authentication |  |    Authorization   |  |
|  |  /swagger     |  |    (JWT)       |  |                     |  |
|  +---------------+  +----------------+  +--------------------+  |
+----------------------------+-------------------------------------+
                             |
                             v
+------------------------------------------------------------------+
|                        Controllers                               |
|                                                                  |
|  +--------------------+    +--------------------------------+   |
|  |  AuthController    |    |  WeatherForecastController      |   |
|  |                    |    |                                 |   |
|  |  POST /register    |    |  GET /weatherforecast          |   |
|  |  POST /login       |    |                                 |   |
|  +--------+-----------+    +---------------+--------------+   |
|           |                                |                   |
+-----------|--------------------------------|-------------------+
            |                                |
            v                                v
+------------------------------------------------------------------+
|                         Services                                 |
|                                                                  |
|  +----------------+                                               |
|  |  IUserService  |<----------------------+                    |
|  +----------------+                         |                    |
|           ^                                 |                    |
|           |                                 |                    |
|  +--------+-----------+                     |                    |
|  | InMemoryUserService|                    |                    |
|  |                    |                     |                    |
|  | - List<User>       |                     |                    |
|  | - GetByUsername() |                     |                    |
|  | - Create()         |                     |                    |
|  +--------------------+                     |                    |
|                                             |                    |
+---------------------------------------------|-------------------+
                                              |
                                              v
+------------------------------------------------------------------+
|                         Models                                    |
|                                                                  |
|  +-------------+    +------------------+    +-----------------+  |
|  |    User     |    | WeatherForecast  |    |    (Future)     |  |
|  +-------------+    +------------------+    +-----------------+  |
|  | - Id        |    | - Date          |                        |
|  | - Username  |    | - TemperatureC  |                        |
|  | - Email     |    | - TemperatureF  |                        |
|  | - PasswordHash    | - Summary      |                        |
|  +-------------+    +------------------+                        |
+------------------------------------------------------------------+
                             |
                             v
+------------------------------------------------------------------+
|                         DTOs                                      |
|                                                                  |
|  +----------------+  +----------------+  +-------------------+  |
|  | RegisterRequest|  |  LoginRequest  |  |   AuthResponse    |  |
|  +----------------+  +----------------+  +-------------------+  |
|  | - Username     |  | - Username     |  | - Token           |  |
|  | - Email        |  | - Password     |  | - Username        |  |
|  | - Password     |  +----------------+  +-------------------+  |
|  +----------------+                                             |
+------------------------------------------------------------------+
                             |
                             v
+------------------------------------------------------------------+
|                    Configuration                                  |
|                                                                  |
|  appsettings.json                                                |
|  +------------------------------------------------------------+  |
|  |  Jwt: { Key, Issuer, Audience }                            |  |
|  |  Logging: { ... }                                          |  |
|  +------------------------------------------------------------+  |
+------------------------------------------------------------------+
```

## Project File Structure

```
opencode-demo/
|
+--- .git/
|    +--- ...
|
+--- WeatherAPI/
|    |
|    +--- Controllers/
|    |    +--- AuthController.cs          # POST /api/auth/register, /api/auth/login
|    |    +--- WeatherForecastController.cs  # GET /api/weatherforecast
|    |
|    +--- Services/
|    |    +--- UserService.cs             # IUserService, InMemoryUserService
|    |
|    +--- Models/
|    |    +--- User.cs                    # User entity
|    |    +--- WeatherForecast.cs         # Weather data model
|    |
|    +--- DTOs/
|    |    +--- RegisterRequest.cs         # Registration request DTO
|    |    +--- LoginRequest.cs            # Login request DTO
|    |    +--- AuthResponse.cs            # Auth response DTO
|    |
|    +--- appsettings.json                # App configuration
|    +--- JwtAuthApi.csproj               # Project file
|    +--- Program.cs                      # Application entry point
|
+--- opencode-demo.sln                     # Solution file
+--- AGENTS.md                            # This file
+--- README.md                            # Documentation
+--- test-auth.http                       # HTTP test file for auth
+--- test-api.http                        # HTTP test file for API
+--- JwtAuthApi.http                      # VS Code HTTP client
```

## API Endpoints Summary

```
+--------------------+------------------------+--------------------------+
| Method | Endpoint         | Controller           | Description             |
+--------+-----------------+---------------------+------------------------+
| POST   | /api/auth/register  | AuthController     | Register new user      |
| POST   | /api/auth/login     | AuthController     | Login & get JWT token  |
| GET    | /api/weatherforecast| WeatherForecastController | Get weather data |
+--------+-----------------+---------------------+------------------------+
```

## Authentication Flow

```
Client                        API                          JWT
  |                            |                            |
  |  1. POST /register         |                            |
  |  {username, email, pwd}   |                            |
  |--------------------------->|                            |
  |                            |  2. Hash password         |
  |                            |  3. Store user in-memory   |
  |                            |<---------------------------|
  |                            |  200 OK                    |
  |                            |                            |
  |  4. POST /login           |                            |
  |  {username, password}     |                            |
  |--------------------------->|                            |
  |                            |  5. Verify password        |
  |                            |  6. Generate JWT token     |
  |                            |<---------------------------|
  |                            |  200 OK + JWT token        |
  |                            |                            |
  |  7. GET /resource         |                            |
  |  Authorization: Bearer   |                            |
  |--------------------------->|                            |
  |                            |  8. Validate JWT           |
  |                            |  9. Return resource        |
  |                            |<---------------------------|
  |                            |  200 OK                    |
```
