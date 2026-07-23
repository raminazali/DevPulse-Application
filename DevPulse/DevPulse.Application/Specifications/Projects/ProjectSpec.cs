using Ardalis.Specification;
using DevPulse.Application.Common;
using DevPulse.Core.Entities.Projects;

namespace DevPulse.Application.Specifications.Projects;

public class ProjectSpec : Specification<Project>
{
    public ProjectSpec(Guid? userId, bool isAdmin, PaginationRequest pagination)
    {
        Query.AsNoTracking()
            .Include(x => x.User).Include(c => c.ErrorLogs)
            .Where(x => isAdmin ? true : x.IsActive && (userId == null || x.UserId == userId))
            .OrderByDescending(x => x.CreatedAt);

        if (pagination.PageSize > 0)
            Query.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize);
    }
}

public class ProjectCountSpec : Specification<Project>
{
    public ProjectCountSpec(Guid? userId, bool isAdmin)
    {
        Query.AsNoTracking().Include(x => x.User)
            .Where(x => isAdmin ? true : x.IsActive && (userId == null || x.UserId == userId))
            .OrderByDescending(x => x.CreatedAt);
    }
}
