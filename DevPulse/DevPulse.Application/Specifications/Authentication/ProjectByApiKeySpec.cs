using Ardalis.Specification;
using DevPulse.Core.Entities.Projects;

namespace DevPulse.Application.Specifications.Authentication;

public sealed class ProjectByApiKeySpec : Specification<Project>
{
    public ProjectByApiKeySpec(string apiKey)
    {
        Query.Where(p => p.ApiKey == apiKey);
    }
}
