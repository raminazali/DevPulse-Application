# Repository Guidelines

## Project Structure & Module Organization
- `DevPulse.Api`: ASP.NET Core Web API project (presentation layer)
- `DevPulse.Application`: Application layer (use cases, DTOs, MediatR handlers)
- `DevPulse.Core`: Domain layer (entities, interfaces, enums)
- `DevPulse.Infrastructure`: Infrastructure layer (EF Core, external services)
- Test projects mirror each: `[Project].Test` (xUnit, Moq, coverlet)
- Configuration files: `.github/` (GitHub Actions), `.claude/` (Codex settings)

## Build, Test, and Development Commands
- **Build**: `dotnet build` (solution-wide)
- **Run API**: `dotnet run --project DevPulse.Api`
- **Run tests**: `dotnet test` (collects coverage via coverlet)
- **Run specific test project**: `dotnet test DevPulse.Api.Test`
- **Format**: Ensure `.editorconfig` settings are respected (IDE format on save)
- **Lint**: No separate linter; rely on compiler warnings and Roslyn analyzers

## Coding Style & Naming Conventions
- Follow .NET naming conventions (PascalCase for public members, camelCase for private)
- Indentation: 4 spaces (tabs not used)
- File names match class names (one public class per file preferred)
- Async methods suffixed with `Async`
- Interfaces prefixed with `I` (e.g., `IRepository`)
- Use `var` for local variables when type is obvious
- Constants: `PascalCase` (or `camelCase` for local?)
- Nullable reference types enabled (`<Nullable>enable</Nullable>`)

## Testing Guidelines
- Framework: xUnit (attribute `[Fact]`, `[Theory]`)
- Mocking: Moq (prefer `Mock.Of<T>` for simple mocks)
- Naming: `MethodName_StateUnderTest_ExpectedBehavior`
- Place tests in `{Project}.Test` matching folder structure
- Aim for 80%+ line coverage (checked via `dotnet test` /coverlet)
- Integration tests: Use `Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory`

## Commit & Pull Request Guidelines
- **Commit messages**: Conventional Commits style (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`)
  - Example: `feat(api): add endpoint for user registration`
- **Pull Requests**:
  - Target `main` branch
  - Include concise title and description
  - Link to related issue (if any) using `Closes #` or `Refs #`
  - Add screenshots for UI changes
  - Ensure all tests pass and coverage does not drop significantly
  - Request at least one review from maintainers

## Architecture Overview
- Clean Architecture (Onion/Hexagonal) separation: Core (domain) independent, Infrastructure and Application depend on Core, API depends on Application and Infrastructure.
- Dependency Injection via .NET built-in container (`Program.cs` in Api)
- Entity Framework Core for data access (providers in Infrastructure)
- MediatR (likely) for decoupling application entry points from business logic
