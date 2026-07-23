using Ardalis.Specification;
using DevPulse.Application.DTOs.Errors;
using DevPulse.Core.Entities.Errors;

namespace DevPulse.Application.Specifications.Errors;

/// <summary>
/// Returns a page of ErrorLog records matching the given filter, newest first.
/// </summary>
public sealed class ErrorsLogSpec : Specification<ErrorLog>
{
    public ErrorsLogSpec(ErrorFilterOptions filter , Guid userId , bool isAdmin)
    {
        Query.Include(e => e.Project).Where(c => isAdmin ? true : c.UserId == userId);
        ApplyFilters(filter);

        Query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);
    }

    private void ApplyFilters(ErrorFilterOptions filter)
    {
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