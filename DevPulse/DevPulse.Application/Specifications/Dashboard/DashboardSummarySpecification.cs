using Ardalis.Specification;
using DevPulse.Core.Entities.Errors;

namespace DevPulse.Application.Specifications.Dashboard;

public sealed class DashboardSummarySpecification : Specification<ErrorLog>
{
    public DashboardSummarySpecification(Guid? ownerId, bool isAdmin)
    {
        Query.AsNoTracking();

        if (!isAdmin)
        {
            Query.Where(x => x.Project.UserId == ownerId);
        }
    }
}