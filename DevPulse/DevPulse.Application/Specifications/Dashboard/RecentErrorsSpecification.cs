using Ardalis.Specification;
using DevPulse.Core.Entities.Errors;

namespace DevPulse.Application.Specifications.Dashboard;

public sealed class RecentErrorsSpecification : Specification<ErrorLog>
{
    public RecentErrorsSpecification(bool isAdmin, Guid? ownerId, int take)
    {
        Query
            .AsNoTracking()
            .Include(x => x.Project)
            .OrderByDescending(x => x.CreatedAt)
            .Take(take);

        if (!isAdmin)
        {
            Query.Where(x => x.Project.UserId == ownerId);
        }
    }
}