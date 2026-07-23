namespace DevPulse.Application.DTOs.Errors;

public class ErrorFilterOptions
{
    public Guid? ProjectId { get; set; }
    public string? ExceptionType { get; set; }
    public string? Url { get; set; }
    public string? Message { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}