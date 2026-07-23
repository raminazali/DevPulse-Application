using Ardalis.Specification;
using DevPulse.Core.Entities.Errors;

namespace DevPulse.Application.Specifications.Dashboard;

/// <summary>
/// Matches ErrorLogs created on/after <paramref name="from"/> and, if given,
/// strictly before <paramref name="to"/>. Used for all the count-based
/// summary/comparison numbers so we don't have to pull full entities into
/// memory just to count them.
/// </summary>
public sealed class ErrorsInRangeSpecification : Specification<ErrorLog>
{
    public ErrorsInRangeSpecification(DateTime from, DateTime? to, Guid? ownerId, bool isAdmin)
    {
        Query.AsNoTracking().Where(x => x.CreatedAt >= from);

        if (to.HasValue)
        {
            Query.Where(x => x.CreatedAt < to.Value);
        }

        if (!isAdmin)
        {
            Query.Where(x => x.Project.UserId == ownerId);
        }
    }
}