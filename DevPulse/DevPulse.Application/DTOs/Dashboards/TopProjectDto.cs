namespace DevPulse.Application.DTOs.Dashboards;

public class TopProjectDto
{
    public Guid ProjectId { get; init; }

    public string ProjectName { get; init; } = default!;

    public long ErrorCount { get; init; }
}
