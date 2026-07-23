namespace DevPulse.Application.DTOs.Dashboards;

public class DashboardSummaryDto
{
    public long TotalErrors { get; init; }

    public long TodayErrors { get; init; }

    public long WeekErrors { get; init; }

    public long MonthErrors { get; init; }

    public int ActiveProjects { get; init; }

    public DateTime? LastErrorAt { get; init; }
}
