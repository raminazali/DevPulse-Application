# DevPulse

DevPulse is an error tracking and monitoring platform designed to help development teams capture, track, and resolve application errors efficiently. The system provides a RESTful API for reporting errors, viewing error details, managing projects and users, and monitoring error trends via a dashboard.

## Table of Contents

- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Setup](#setup)
  - [Running the API](#running-the-api)
  - [Running Tests](#running-tests)
- [API Endpoints](#api-endpoints)
  - [Authentication](#authentication)
  - [Users (Admin)](#users-admin)
  - [Projects](#projects)
  - [Errors](#errors)
  - [External Errors (API Key)](#external-errors-api-key)
  - [Dashboard](#dashboard)
- [Authentication & Authorization](#authentication--authorization)
  - [JWT Bearer](#jwt-bearer)
  - [API Key Authentication](#api-key-authentication)
- [Configuration](#configuration)
  - [App Settings](#app-settings)
  - [JWT Settings](#jwt-settings)
  - [S3 Settings](#s3-settings)
- [Architecture Details](#architecture-details)
  - [Clean Architecture](#clean-architecture)
  - [Exception Handling](#exception-handling)
  - [Rate Limiting](#rate-limiting)
  - [Caching](#caching)
  - [Security Headers](#security-headers)
- [Contributing](#contributing)
- [License](#license)

## Project Overview

DevPulse provides a backend service for error tracking with the following core capabilities:

- **Error Reporting**: Report errors from your applications via a public API key-secured endpoint.
- **Error Monitoring**: Browse, filter, and inspect error details through a paginated API.
- **Project Management**: Organize errors by projects, each with a unique API key for secure ingestion.
- **User Management**: Admin-only user creation and management with role-based access control.
- **Authentication**: JWT-based login for dashboard users; API key-based authentication for external error ingestion.
- **Dashboard**: Aggregated error statistics, trends, and top exception types.
- **Global Exception Handling**: Centralized exception handling returning RFC 7807 Problem Details responses.
- **Rate Limiting**: Protect authentication and error creation endpoints from abuse.
- **Security Headers**: Content-Security-Policy, X-Frame-Options, Referrer-Policy, and more.

## Architecture

DevPulse follows a clean/onion architecture with strict separation of concerns:

```
src/
├── DevPulse.Api                # Presentation layer (Controllers, Middleware, Authentication)
├── DevPulse.Application        # Application layer (DTOs, Services, Specifications, Interfaces)
├── DevPulse.Core               # Domain layer (Entities, Enums, Exceptions, Interfaces)
└── DevPulse.Infrastructure     # Infrastructure layer (EF Core DbContext, Repository, AWS S3)
tests/
├── DevPulse.Api.Test
├── DevPulse.Application.Test
├── DevPulse.Core.Test
└── DevPulse.Infrastructure.Test
```

### Key Architectural Patterns

- **Clean Architecture**: Each layer has distinct responsibilities; dependencies flow inward (Core has no dependencies).
- **Repository Pattern**: Generic `IRepository<T>` backed by Ardalis.Specification.EntityFrameworkCore.
- **Specification Pattern**: All query logic is encapsulated in reusable `Specification<T>` classes.
- **Service Pattern**: Business logic is organized into injectable services (no MediatR).
- **DTO Pattern**: Data Transfer Objects decouple internal entities from API contracts.
- **Global Exception Handling**: Custom `IExceptionHandler` maps domain exceptions to RFC 7807 ProblemDetails.
- **Dual Authentication**: JWT Bearer for dashboard users, API Key for external error ingestion.

## Technology Stack

- **Framework**: .NET 10.0 (ASP.NET Core Web API)
- **Language**: C# 13
- **ORM**: Entity Framework Core 10.x with Npgsql (PostgreSQL)
- **Database**: PostgreSQL
- **Authentication**: JWT Bearer tokens + Custom API Key authentication
- **Password Hashing**: ASP.NET Core Identity's `IPasswordHasher<T>`
- **Specification Library**: Ardalis.Specification v9.3.1
- **API Documentation**: OpenAPI with Scalar UI
- **Logging**: Built-in `Microsoft.Extensions.Logging`
- **Caching**: In-memory caching (`IMemoryCache`)
- **Rate Limiting**: `System.Threading.RateLimiting` (fixed window)
- **Security Headers**: NetEscapades.AspNetCore.SecurityHeaders
- **Cloud Storage**: AWS S3 via AWSSDK.S3 (implemented, configurable)
- **Testing**: xUnit, Moq, FluentAssertions

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL Server](https://www.postgresql.org/download/)
- [AWS Account](https://aws.amazon.com/) (for S3 screenshot storage - optional)
- [Git](https://git-scm.com/)

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd DevPulse
   ```

2. **Configure the application**
   - Copy `appsettings.example.json` to `appsettings.Development.json` (or update `appsettings.json`)
   - Update the connection strings:
     ```json
     {
       "ConnectionStrings": {
         "DevPulse": "Host=localhost;Port=5432;Database=DevPulse;Username=postgres;Password=your_password"
       },
       "JwtSettings": {
         "Key": "your_super_secret_key_here_at_least_32_chars",
         "Issuer": "DevPulse",
         "Audience": "DevPulse"
       },
       "S3Settings": {
         "BucketName": "your-bucket-name",
         "Region": "your-region",
         "AccessKey": "your-access-key",
         "SecretKey": "your-secret-key"
       }
     }
     ```
   - For local development without AWS, the S3 service can be left unconfigured.

3. **Apply database migrations**
   ```bash
   dotnet ef database update --project DevPulse.Infrastructure --startup-project DevPulse.Api
   ```

### Running the API

```bash
dotnet run --project DevPulse.Api
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

Scalar API reference UI will be available at `/scalar` (development mode only).

### Running Tests

```bash
dotnet test
```

To run tests with coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## API Endpoints

### Authentication

Public endpoints for obtaining JWT tokens.

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `POST` | `/api/auth/login` | Authenticate user and return JWT token | None |

**Rate Limited:** 5 requests per minute per IP.

### Users (Admin)

Admin-only endpoints for managing users.

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `POST` | `/api/users` | Create a new user | Admin |
| `GET`  | `/api/users` | Get paginated list of users | Admin |
| `GET`  | `/api/users/{id}` | Get user by ID | Admin |

### Projects

Endpoints for managing error tracking projects.

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET`  | `/api/projects/{id}` | Get project by ID | Admin, User |
| `GET`  | `/api/projects/user` | Get projects for the current user (paginated) | Admin, User |
| `POST` | `/api/projects` | Create a new project | Admin, User |
| `PUT`  | `/api/projects` | Update an existing project | Admin, User |
| `DELETE` | `/api/projects/{id}` | Delete a project | Admin, User |

### Errors

Endpoints for browsing and inspecting reported errors.

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET`  | `/api/errors/{id}` | Get error detail by ID | Admin, User |
| `GET`  | `/api/errors` | Get paginated list of errors with filtering | Admin, User |

**Query Parameters for `GET /api/errors`:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `ProjectId` | Guid | Filter by project |
| `ExceptionType` | string | Filter by exception type |
| `Page` | int | Page number (default: 1) |
| `PageSize` | int | Items per page (default: 20) |
| `Search` | string | Search in error message |

### External Errors (API Key)

Public endpoint for reporting errors from external applications. Secured via API key authentication.

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `POST` | `/api/external/errors` | Report a new error from an external application | API Key |

**Rate Limited:** 25 requests per minute per IP.

**Headers:**
| Header | Description |
|--------|-------------|
| `X-Api-Key` | The project's unique API key |

**Request Body:**
```json
{
  "message": "NullReferenceException",
  "stackTrace": "at MyApp.Services.UserService.GetUser()",
  "url": "https://myapp.com/users",
  "exceptionType": "System.NullReferenceException",
  "method": "GET",
  "requestBody": "{\"userId\": 123}",
  "queryString": "?page=1",
  "userId": "user-123",
  "browser": "Chrome 120",
  "ipAddress": "192.168.1.1"
}
```

The `ProjectId` is automatically resolved from the API key — it should not be included in the request body.

### Dashboard

Endpoints for aggregated error monitoring and analytics.

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET`  | `/api/dashboard` | Get dashboard report with error statistics and trends | Admin, User |

**Query Parameters for `GET /api/dashboard`:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `recentErrorsTake` | int | 10 | Number of recent errors to include |
| `topExceptionTypesTake` | int | 10 | Number of top exception types |
| `topUsersTake` | int | 10 | Number of top users by error count |
| `topProjectsTake` | int | 10 | Number of top projects by error count |

The dashboard response is cached in memory for 3 minutes.

## Authentication & Authorization

DevPulse uses a dual authentication system:

### JWT Bearer

Default authentication scheme for dashboard endpoints. Tokens are obtained via `POST /api/auth/login` and must be included in request headers:

```
Authorization: Bearer <token>
```

JWT tokens contain the following claims:
- `ClaimTypes.NameIdentifier`: User's GUID
- `ClaimTypes.Role`: `Admin` or `User`
- `ClaimTypes.Email`: User's email
- Standard JWT claims (`iss`, `aud`, `exp`, `iat`)

### API Key Authentication

Used exclusively by the external error ingestion endpoint (`POST /api/external/errors`). API keys are unique per project and are passed via the `X-Api-Key` header. The system looks up the project by API key and creates claims for `ProjectId` and `ProjectName`.

## Configuration

### App Settings

The `appsettings.json` file contains configuration for:

- **ConnectionStrings**: Database connection string (key: `DevPulse`)
- **JwtSettings**: JWT token configuration (key, issuer, audience)
- **S3Settings**: AWS S3 configuration for screenshot storage
- **Logging**: Log levels and provider configuration

### JWT Settings

| Setting | Description |
|---------|-------------|
| `Key` | Secret key for token signing (minimum 32 characters) |
| `Issuer` | Token issuer (e.g., `DevPulse`) |
| `Audience` | Token audience (e.g., `DevPulse`) |

Token expiration is 24 hours (set in `AuthService`).

### S3 Settings

Error screenshots can be stored in AWS S3. The service supports file upload, retrieval, deletion, and existence checks.

| Setting | Description |
|---------|-------------|
| `BucketName` | S3 bucket name |
| `Region` | AWS region (e.g., `us-east-1`) |
| `AccessKey` | AWS access key ID |
| `SecretKey` | AWS secret access key |

The S3 service implementation is complete but must be enabled in DI registration (`InfrastructureExtention.cs`).

## Architecture Details

### Clean Architecture

```
┌─────────────────────────────────────────────────┐
│                  DevPulse.Api                     │
│  Controllers, Middleware, Auth Handlers, Config   │
├─────────────────────────────────────────────────┤
│              DevPulse.Application                 │
│  DTOs, Service Interfaces & Implementations,      │
│  Specifications                                  │
├─────────────────────┬───────────────────────────┤
│   DevPulse.Core     │   DevPulse.Infrastructure  │
│   Entities          │   EF Core DbContext         │
│   Interfaces        │   Repository Implementation │
│   Exceptions        │   AWS S3, Caching           │
│   Enums             │   Migrations                │
└─────────────────────┴───────────────────────────┘
```

### Exception Handling

A global exception handler (`GlobalExceptionHandler`) implements `IExceptionHandler` and maps custom domain exceptions to RFC 7807 ProblemDetails responses:

| Exception | HTTP Status | Problem Code |
|-----------|-------------|--------------|
| `NotFoundException` | 404 | Custom from exception |
| `UnauthorizedException` | 401 | Custom from exception |
| `ForbiddenException` | 403 | Custom from exception |
| `ValidationException` | 400 | Custom + validation errors |
| `BusinessRuleException` | 400 | Custom from exception |
| `ArgumentException` | 400 | `VALIDATION_ERROR` |
| Unhandled exceptions | 500 | `Server Error` |

### Rate Limiting

Fixed window rate limiting is applied to sensitive endpoints:

| Policy | Endpoint | Limit | Window |
|--------|----------|-------|--------|
| `LoginPolicy` | `POST /api/auth/login` | 5 requests | 1 minute |
| `ErrorCreatePolicy` | `POST /api/external/errors` | 25 requests | 1 minute |

Exceeded requests receive a `429 Too Many Requests` response.

### Caching

The dashboard report is cached in-memory with a 3-minute time-to-live (TTL) using `IMemoryCache`. The cache service (`ICacheService`/`CacheService`) is available for future caching needs.

### Security Headers

The following security headers are applied to all responses (via `NetEscapades.AspNetCore.SecurityHeaders`):

| Header | Value |
|--------|-------|
| `X-Content-Type-Options` | `nosniff` |
| `X-Frame-Options` | `SAMEORIGIN` |
| `Content-Security-Policy` | `default-src 'self'` |
| `Referrer-Policy` | `strict-origin-when-cross-origin` |
| `Permissions-Policy` | `camera=(), microphone=(), geolocation=()` |

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -am 'feat: add some feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please ensure your code follows the existing coding standards, includes appropriate unit tests, and maintains or improves code coverage.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

*Last updated: July 22, 2026*
