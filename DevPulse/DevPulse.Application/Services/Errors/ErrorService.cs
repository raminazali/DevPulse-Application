using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Errors;
using DevPulse.Application.Services.Errors.Inetfaces;
using DevPulse.Application.Specifications.Errors;
using DevPulse.Application.Utilities;
using DevPulse.Core.Abstractions.Configuration;
using DevPulse.Core.Entities.Errors;
using DevPulse.Core.Entities.Projects;
using DevPulse.Core.Exceptions;
using DevPulse.Infrastructure.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevPulse.Application.Services.Errors;

/// <summary>
/// Every incoming exception is stored as one independent ErrorLog row.
/// There is no grouping, fingerprinting, or occurrence tracking.
/// </summary>
public class ErrorService : IErrorService
{
    private readonly IRepository<ErrorLog> _errorLogRepo;
    private readonly IRepository<Project> _projectRepository;
    //private readonly IS3StorageService _s3StorageService;
    private readonly IOptions<S3Settings> _s3Settings;
    private readonly ILogger<ErrorService> _logger;

    public ErrorService(
        IRepository<ErrorLog> errorLogRepo,
        //IS3StorageService s3StorageService,
        IOptions<S3Settings> s3Settings,
        ILogger<ErrorService> logger,
        IRepository<Project> projectRepository)
    {
        _errorLogRepo = errorLogRepo ?? throw new ArgumentNullException(nameof(errorLogRepo));
        //_s3StorageService = s3StorageService ?? throw new ArgumentNullException(nameof(s3StorageService));
        _s3Settings = s3Settings ?? throw new ArgumentNullException(nameof(s3Settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Creates one standalone ErrorLog. No fingerprinting, no lookup, no merge —
    /// every call inserts a new row, optionally with a screenshot attached.
    /// </summary>
    public async Task<ErrorDetailDto> CreateErrorAsync(CreateErrorRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);

        if(project is null)
            throw new NotFoundException("پروژه مورد نظر یافت نشد!" , "PROJECT_NOT_FOUND");

        var errorLog = ErrorLog.Create(
            request.ProjectId,
            request.Message,
            request.StackTrace,
            request.Url,
            request.ExceptionType,
            request.Method,
            request.RequestBody,
            request.QueryString,
            project.UserId,
            request.Browser,
            request.IpAddress,
            DateTime.UtcNow);

        await _errorLogRepo.AddAsync(errorLog, cancellationToken);
        await _errorLogRepo.SaveChangesAsync(cancellationToken);

        return MapToDetailDto(errorLog);
    }

    /// <summary>
    /// Loads a single ErrorLog by id. Read-only — nothing is updated or mutated.
    /// </summary>
    public async Task<ErrorDetailDto> GetErrorByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var spec = new ErrorByIdSpec(id);
        var errorLog = await _errorLogRepo.FirstOrDefaultAsync(spec, cancellationToken);

        if (errorLog == null)
            throw new NotFoundException("خطا مورد نظر یافت نشد!", "ERROR_NOT_FOUND");

        return MapToDetailDto(errorLog);
    }

    /// <summary>
    /// Returns a paged list of ErrorLog records matching the given filter.
    /// </summary>
    public async Task<PagedResult<ErrorListItemDto>> GetErrorsAsync(ErrorFilterOptions filter ,Guid userId , bool isAdmin, CancellationToken cancellationToken = default)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));

        var spec = new ErrorsLogSpec(filter, userId ,isAdmin);
        var countSpec = new ErrorsCountSpec(filter, userId , isAdmin);

        var totalCount = await _errorLogRepo.CountAsync(countSpec, cancellationToken);
        var errorLogs = await _errorLogRepo.ListAsync(spec, cancellationToken);

        return new PagedResult<ErrorListItemDto>
        {
            Items = errorLogs.Select(MapToListItemDto).ToList(),
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }


    private ErrorDetailDto MapToDetailDto(ErrorLog errorLog)
    {
        return new ErrorDetailDto
        {
            Id = errorLog.Id,
            ProjectId = errorLog.ProjectId,
            ProjectName = errorLog.Project?.Name ?? string.Empty,
            Message = errorLog.Message,
            StackTrace = errorLog.StackTrace,
            Url = errorLog.Url,
            ExceptionType = errorLog.ExceptionType,
            Method = errorLog.Method,
            RequestBody = errorLog.RequestBody,
            QueryString = errorLog.QueryString,
            Browser = errorLog.Browser,
            IpAddress = errorLog.IpAddress,
            CreatedAt = errorLog.CreatedAt,
        };
    }

    private static ErrorListItemDto MapToListItemDto(ErrorLog errorLog)
    {
        return new ErrorListItemDto
        {
            Id = errorLog.Id,
            ProjectId = errorLog.ProjectId,
            ProjectName = errorLog.Project?.Name ?? string.Empty,
            Message = errorLog.Message,
            StackTrace = errorLog.StackTrace,
            Url = errorLog.Url,
            ExceptionType = errorLog.ExceptionType,
            Method = errorLog.Method,
            Browser = errorLog.Browser,
            IpAddress = errorLog.IpAddress,
            CreatedAt = errorLog.CreatedAt
        };
    }
}