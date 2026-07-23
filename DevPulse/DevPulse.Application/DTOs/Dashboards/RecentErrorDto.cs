namespace DevPulse.Application.DTOs.Dashboards;

public sealed record RecentErrorDto
{
    public Guid ErrorId { get; init; }

    public Guid ProjectId { get; init; }

    public string ProjectName { get; init; } = default!;

    public string ExceptionType { get; init; } = default!;

    public DateTime CreatedAt { get; init; }
}
