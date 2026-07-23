using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Errors;

namespace DevPulse.Application.Services.Errors.Inetfaces;

public interface IErrorService
{
    /// <summary>
    /// Creates one standalone ErrorLog record. No lookup or merge happens first.
    /// </summary>
    Task<ErrorDetailDto> CreateErrorAsync(CreateErrorRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a single ErrorLog by id. Throws NotFoundException if it doesn't exist.
    /// </summary>
    Task<ErrorDetailDto> GetErrorByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paged list of ErrorLog records matching the given filter.
    /// </summary>
    Task<PagedResult<ErrorListItemDto>> GetErrorsAsync(ErrorFilterOptions filter, Guid userId, bool isAdmin, CancellationToken cancellationToken = default);
}