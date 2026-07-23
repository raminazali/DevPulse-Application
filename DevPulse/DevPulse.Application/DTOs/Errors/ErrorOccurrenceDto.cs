namespace DevPulse.Application.DTOs.Errors;

public class ErrorOccurrenceDto
{
    public Guid Id { get; set; }
    public Guid ErrorId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string RequestBody { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public string StackTrace { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? Browser { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}