# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in the DevPulse repository.

## Development Commands

### Building the Solution
```bash
dotnet build
```

### Running the API
```bash
dotnet run --project DevPulse.Api
```

### Running Tests
```bash
dotnet test
```

### Running Specific Tests
```bash
dotnet test --filter "FullyQualifiedName~Namespace.ClassName.MethodName"
```

### Running with File Watcher (for development)
```bash
dotnet watch run --project DevPulse.Api
```

## Project Structure & Architecture

### Layered Architecture (Clean/Onion)
1. **DevPulse.Api** - Presentation layer (Controllers, API endpoints)
2. **DevPulse.Application** - Application layer (DTOs, Services, Interfaces, Specifications)
3. **DevPulse.Core** - Domain layer (Entities, Enums, Exceptions, Interfaces)
4. **DevPulse.Infrastructure** - Infrastructure layer (Repository implementations, DB context)

### Key Architectural Patterns

#### Exception Handling
- Custom exception hierarchy based on `DevPulseException` base class
- Global exception handler (`GlobalExceptionHandler`) implementing `IExceptionHandler`
- Maps domain exceptions to RFC 7807 ProblemDetails responses
- Security-conscious: Authentication failures always return generic "Invalid email or password" message to prevent user enumeration

#### Authentication & Authorization
- JWT Bearer token authentication
- Password hashing using ASP.NET Core Identity's `IPasswordHasher`
- Role-based authorization (Admin/User roles)
- Token expiration configured in appsettings.json (1 day)

#### Data Access
- Repository pattern with generic `IRepository<T>` interface
- Ardalis.Specification for query specifications
- Extension method `ApplyPaging` for pagination (used in service layer)

#### Validation & Error Messages
- Centralized Persian/Farsi error messages in `ErrorMessages` static class
- Validation through ASP.NET Core's built-in model validation (`[ApiController]` attribute)

## Important Implementation Details

### Services Layer
- Services contain business logic (thick services, thin controllers)
- Services throw domain exceptions rather than returning null/error codes
- Services handle cross-cutting concerns like logging user activity (`RecordLogin`)

### Controllers
- Controllers are minimal and focused on HTTP concerns
- Controllers call services and return appropriate HTTP responses
- No business logic resides in controllers

### Pagination
- All service list methods use `QueryExtensions.ApplyPaging` extension method
- Returns `PagedResult<T>` containing items and pagination metadata
- Note: Current implementation loads all data then pages (not optimal for large datasets)

### Security
- Authentication errors return generic messages to prevent user enumeration
- Passwords are hashed using ASP.NET Core's built-in hasher
- JWT tokens are signed with a secure key from configuration
- HTTPS redirection enabled in production

## Common Development Tasks

### Adding a New Service
1. Create interface in `DevPulse.Application/Services/[Feature]/Interfaces/`
2. Implement service in `DevPulse.Application/Services/[Feature]/`
3. Register service in `DependencyInjection` extensions (ApplicationLayer.cs)
4. Inject into controllers via constructor

### Adding a New API Endpoint
1. Create controller in `DevPulse.Api/Controllers/[Version]/`
2. Inject required services via constructor
3. Implement action methods calling service layer
4. Use appropriate HTTP verbs and status codes

### Adding a New Entity
1. Create entity class in `DevPulse.Core/Entities/[Feature]/`
2. Add corresponding repository methods if needed
3. Create DTOs in `DevPulse.Application/DTOs/[Feature]/` if needed
4. Add specifications in `DevPulse.Application/Specifications/[Feature]/` if needed

### Adding Custom Exceptions
1. Inherit from `DevPulseException` base class
2. Implement constructor with appropriate HTTP status code mapping
3. Use in services when business rules are violated
4. Global handler will automatically map to appropriate ProblemDetails response

## Configuration
- JWT settings in `appsettings.json` under `JwtSettings` section
- Connection strings and other infrastructure settings in `appsettings.json`
- Environment-specific settings in `appsettings.{Environment}.json`

## Code Style
- Follows standard C# naming conventions
- Uses dependency injection extensively
- Services and repositories are injected via constructors
- Async/await used consistently for I/O operations
- Guard clauses for parameter validation