using Ardalis.Specification;
using DevPulse.Core.Entities.Errors;

namespace DevPulse.Application.Specifications.Dashboard;

public sealed class HourlyErrorsSpecification : Specification<ErrorLog>
{
    public HourlyErrorsSpecification(Guid? ownerId, bool isAdmin)
    {
        Query.AsNoTracking();

        if (!isAdmin)
        {
            Query.Where(x => x.Project.UserId == ownerId);
        }
    }
}