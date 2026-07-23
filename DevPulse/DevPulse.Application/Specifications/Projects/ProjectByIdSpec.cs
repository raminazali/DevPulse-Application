using Ardalis.Specification;
using DevPulse.Core.Entities.Projects;

namespace DevPulse.Application.Specifications.Projects;

public class ProjectByIdSpec : Specification<Project>
{
    public ProjectByIdSpec(Guid id, Guid userId, bool isAdmin)
    {
        Query
            .Where(x => x.Id == id && (isAdmin ? true :  x.UserId == userId && x.IsActive))
            .Include(x => x.User);
    }
}
