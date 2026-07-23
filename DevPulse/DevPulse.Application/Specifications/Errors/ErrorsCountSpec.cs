using Ardalis.Specification;
using DevPulse.Application.DTOs.Errors;
using DevPulse.Core.Entities.Errors;

namespace DevPulse.Application.Specifications.Errors;

/// <summary>
/// Counts ErrorLog records matching the given filter (mirrors ErrorsSpec's
/// filters, without paging or includes).
/// </summary>
public sealed class ErrorsCountSpec : Specification<ErrorLog>
{
    public ErrorsCountSpec(ErrorFilterOptions filter, Guid userId, bool isAdmin)
    {
        Query.Where(c => isAdmin ? true : c.UserId == userId);
        if (filter.ProjectId.HasValue)
            Query.Where(e => e.ProjectId == filter.ProjectId.Value);

        if (!string.IsNullOrWhiteSpace(filter.ExceptionType))
            Query.Where(e => e.ExceptionType == filter.ExceptionType);

        if (!string.IsNullOrWhiteSpace(filter.Url))
            Query.Where(e => e.Url.Contains(filter.Url));

        if (!string.IsNullOrWhiteSpace(filter.Message))
            Query.Where(e => e.Message.Contains(filter.Message));

        if (filter.FromDate.HasValue)
            Query.Where(e => e.CreatedAt >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            Query.Where(e => e.CreatedAt <= filter.ToDate.Value);
    }
}