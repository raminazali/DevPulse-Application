namespace DevPulse.Application.DTOs.Dashboards;

public sealed record ErrorComparisonDto
{
    public long Today { get; init; }

    public long Yesterday { get; init; }

    public double TodayPercentage { get; init; }

    public long Week { get; init; }

    public long LastWeek { get; init; }

    public double WeekPercentage { get; init; }

    public long Month { get; init; }

    public long LastMonth { get; init; }

    public double MonthPercentage { get; init; }
}