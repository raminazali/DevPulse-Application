using Microsoft.AspNetCore.Http;

namespace DevPulse.Application.DTOs.Errors;

/// <summary>
/// Request to log a single exception. An optional screenshot may be attached.
/// </summary>
public class CreateErrorRequest
{
    public Guid ProjectId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string RequestBody { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public string? Browser { get; set; }
    public string? IpAddress { get; set; }
}