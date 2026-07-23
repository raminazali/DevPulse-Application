using Ardalis.Specification;
using DevPulse.Core.Entities.Projects;

namespace DevPulse.Application.Specifications.Projects;

public sealed class ActiveProjectsSpecification : Specification<Project>
{
    public ActiveProjectsSpecification(Guid? ownerId, bool isAdmin)
    {
        Query.AsNoTracking().Where(x => x.IsActive);

        if (!isAdmin)
        {
            Query.Where(x => x.UserId == ownerId);
        }
    }
}