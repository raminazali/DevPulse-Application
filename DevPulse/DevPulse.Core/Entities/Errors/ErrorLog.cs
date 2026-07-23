using DevPulse.Core.Entities.Projects;

namespace DevPulse.Core.Entities.Errors;

/// <summary>
/// A single, standalone record of an exception. There is no grouping,
/// fingerprinting, or occurrence tracking — every exception that arrives
/// creates exactly one ErrorLog.
/// </summary>
public class ErrorLog
{
    public Guid Id { get; private set; }

    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    public string Message { get; private set; } = string.Empty;
    public string StackTrace { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public string ExceptionType { get; private set; } = string.Empty;
    public string Method { get; private set; } = string.Empty;
    public string RequestBody { get; private set; } = string.Empty;
    public string? QueryString { get; private set; }
    public Guid? UserId { get; private set; }
    public string? Browser { get; private set; }
    public string? IpAddress { get; private set; }

    public DateTime CreatedAt { get; private set; }

    // EF Core
    private ErrorLog() { }

    public static ErrorLog Create(
        Guid projectId,
        string message,
        string stackTrace,
        string url,
        string exceptionType,
        string method,
        string requestBody,
        string? queryString,
        Guid? userId,
        string? browser,
        string? ipAddress,
        DateTime createdAt)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("شناسه پروژه نامعتبر است.", nameof(projectId));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("پیام خطا نمی‌تواند خالی باشد.", nameof(message));

        return new ErrorLog
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Message = message,
            StackTrace = stackTrace ?? string.Empty,
            Url = url ?? string.Empty,
            ExceptionType = exceptionType ?? string.Empty,
            Method = method ?? string.Empty,
            RequestBody = requestBody ?? string.Empty,
            QueryString = queryString,
            UserId = userId,
            Browser = browser,
            IpAddress = ipAddress,
            CreatedAt = createdAt
        };
    }
}