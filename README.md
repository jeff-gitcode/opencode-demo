# OpenCode Demo Projects

A multi-project demo containing a .NET 8 Web API with JWT authentication and a Next.js frontend.

## Projects

### WeatherAPI (.NET 8)
- ASP.NET Core Web API with JWT authentication
- User registration and login endpoints
- In-memory data storage

### Next-Example (Next.js 14)
- React frontend application
- TypeScript based
- Standalone production build

## Tech Stack

| Project | Framework | Language |
|---------|-----------|----------|
| WeatherAPI | .NET 8.0 | C# |
| Next-Example | Next.js 14 | TypeScript |

## Prerequisites

- .NET 8.0 SDK
- Node.js 20+
- Docker & Docker Compose (for containerized setup)

## Local Development

### WeatherAPI

```bash
# Build
dotnet build WeatherAPI/JwtAuthApi.csproj

# Run
dotnet run --project WeatherAPI/JwtAuthApi.csproj
```

API runs at `http://localhost:5000`

### Next-Example

```bash
# Install dependencies
cd next-example
npm install

# Run development server
npm run dev
```

Frontend runs at `http://localhost:3000`

## Running Tests

```bash
# Run all tests
dotnet test WeatherAPI.Tests/WeatherAPI.Tests.csproj
```

## Running with Docker Compose

```bash
# Build and start all services
docker-compose up --build

# Start only (if already built)
docker-compose up

# Stop all services
docker-compose down
```

### Services

| Service | Port | URL |
|---------|------|-----|
| WeatherAPI | 5000 | http://localhost:5000 |
| Next-Example | 3000 | http://localhost:3000 |

## API Endpoints

### POST /api/auth/register
Register a new user.

```json
{
  "username": "john",
  "email": "john@example.com",
  "password": "password123"
}
```

### POST /api/auth/login
Login with existing credentials.

```json
{
  "username": "john",
  "password": "password123"
}
```

Response:
```json
{
  "token": "eyJhbG...",
  "username": "john"
}
```

## CI/CD

GitHub Actions workflow configured at `.github/workflows/CI.yaml`.

Triggers on push/PR to `main` or `master`.

Runs:
- .NET build and tests
- Next.js build and lint
