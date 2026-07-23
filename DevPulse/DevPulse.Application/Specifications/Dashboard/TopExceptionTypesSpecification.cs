using Ardalis.Specification;
using DevPulse.Core.Entities.Errors;

namespace DevPulse.Application.Specifications.Dashboard;

public sealed class TopExceptionTypesSpecification : Specification<ErrorLog>
{
    public TopExceptionTypesSpecification(Guid? ownerId, bool isAdmin)
    {
        Query
            .AsNoTracking()
            .Where(x => !string.IsNullOrWhiteSpace(x.ExceptionType));

        if (!isAdmin)
        {
            Query.Where(x => x.Project.UserId == ownerId);
        }
    }
}