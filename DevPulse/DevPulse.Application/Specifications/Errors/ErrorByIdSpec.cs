using Ardalis.Specification;
using DevPulse.Application.DTOs.Errors;
using DevPulse.Core.Entities.Errors;

namespace DevPulse.Application.Specifications.Errors;

/// <summary>
/// Returns a page of ErrorLog records matching the given filter, newest first.
/// </summary>
public sealed class ErrorByIdSpec : Specification<ErrorLog>
{
    public ErrorByIdSpec(Guid id)
    {
        Query
            .Where(e => e.Id == id)
            .Include(e => e.Project);
    }
}