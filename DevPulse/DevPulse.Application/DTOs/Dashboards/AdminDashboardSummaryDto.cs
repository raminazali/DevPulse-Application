namespace DevPulse.Application.DTOs.Dashboards;

public sealed record AdminDashboardSummaryDto
{
    public int TotalUsers { get; init; }

    public int ActiveUsers { get; init; }

    public int TotalProjects { get; init; }

    public int ActiveProjects { get; init; }

    public long TotalErrors { get; init; }

    public long TodayErrors { get; init; }
}

public sealed record TopUserDto
{
    public Guid UserId { get; init; }

    public string FullName { get; init; } = default!;

    public int ProjectCount { get; init; }

    public long ErrorCount { get; init; }
}

public sealed record AdminTopProjectDto
{
    public Guid ProjectId { get; init; }

    public string ProjectName { get; init; } = default!;

    public string OwnerName { get; init; } = default!;

    public long ErrorCount { get; init; }
}