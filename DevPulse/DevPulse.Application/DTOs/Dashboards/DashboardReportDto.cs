namespace DevPulse.Application.DTOs.Dashboards;

/// <summary>
/// Everything the dashboard page needs, fetched in a single round trip.
/// AdminSummary and AdminTopProjects are only populated when the caller is
/// an Admin — null otherwise, so a "User" role never even sees the shape
/// of the admin-only data.
/// </summary>
public sealed record DashboardReportDto
{
    public DashboardSummaryDto Summary { get; init; } = default!;

    public ErrorComparisonDto Comparison { get; init; } = default!;

    public List<RecentErrorDto> RecentErrors { get; init; } = new();

    public List<KeyValueDto> TopExceptionTypes { get; init; } = new();

    public List<TopUserDto> TopUsers { get; init; } = new();

    public List<KeyValueDto> HourlyErrors { get; init; } = new();

    public List<KeyValueDto> ProjectErrorDistribution { get; init; } = new();

    public List<KeyValueDto> UserErrorDistribution { get; init; } = new();

    public AdminDashboardSummaryDto? AdminSummary { get; init; }

    public List<AdminTopProjectDto>? AdminTopProjects { get; init; }
}
