namespace DevPulse.Application.DTOs.Errors;

public class ErrorDto
{
    public Guid Id { get; set; }
    public string Fingerprint { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ExceptionType { get; set; }
    public string? Url { get; set; }
    public int Occurrences { get; set; }
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}