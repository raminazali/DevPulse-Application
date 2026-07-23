namespace DevPulse.Application.DTOs.Errors;

/// <summary>
/// Full detail of a single ErrorLog, returned on create and get-by-id.
/// </summary>
public class ErrorDetailDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string RequestBody { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public string? Browser { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}