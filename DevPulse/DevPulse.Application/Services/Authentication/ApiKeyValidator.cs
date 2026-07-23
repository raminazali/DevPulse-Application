using DevPulse.Core.Entities.Projects;
using DevPulse.Infrastructure.Repository.Contracts;
using DevPulse.Application.Specifications.Authentication;

namespace DevPulse.Infrastructure.Services.Authentication;

/// <summary>
/// Result of validating an external API key: which project it belongs to.
/// </summary>
public record ApiKeyPrincipal(Guid ProjectId, string ProjectName);
public interface IApiKeyValidator
{
    /// <summary>
    /// Looks up the given API key. Returns null if it doesn't match any project
    /// (unknown, revoked, etc.) — the caller should treat that as unauthenticated.
    /// </summary>
    Task<ApiKeyPrincipal?> ValidateAsync(string apiKey, CancellationToken cancellationToken = default);
}

public class ApiKeyValidator : IApiKeyValidator
{
    private readonly IRepository<Project> _projectRepo;

    public ApiKeyValidator(IRepository<Project> projectRepo)
    {
        _projectRepo = projectRepo ?? throw new ArgumentNullException(nameof(projectRepo));
    }

    public async Task<ApiKeyPrincipal?> ValidateAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return null;

        var spec = new ProjectByApiKeySpec(apiKey);
        var project = await _projectRepo.FirstOrDefaultAsync(spec, cancellationToken);

        return project == null
            ? null
            : new ApiKeyPrincipal(project.Id, project.Name);
    }
}
