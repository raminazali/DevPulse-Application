using Ardalis.Specification;
using DevPulse.Core.Entities.Errors;

namespace DevPulse.Application.Specifications.Dashboard;

public sealed class TopProjectsSpecification : Specification<ErrorLog>
{
    public TopProjectsSpecification(bool isAdmin, Guid? ownerId)
    {
        Query
            .AsNoTracking()
            .Include(x => x.Project)
            .ThenInclude(x => x.User); 

        if (!isAdmin)
        {
            Query.Where(x => x.Project.UserId == ownerId);
        }
    }
}