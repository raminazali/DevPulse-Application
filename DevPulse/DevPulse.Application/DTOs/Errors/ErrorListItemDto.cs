namespace DevPulse.Application.DTOs.Errors;

/// <summary>
/// Lightweight projection of an ErrorLog for list/paged views.
/// </summary>
public class ErrorListItemDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string? Browser { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}