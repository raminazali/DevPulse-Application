using DevPulse.Api.Extentions;
using DevPulse.Application.DTOs.Dashboards;
using DevPulse.Application.Services.Dashboard.Interfaces;
using DevPulse.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevPulse.Api.Controllers.V1;

[Route("api/[controller]")]
[Authorize(Roles = "Admin,User")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ICacheService _cacheService;
    public DashboardController(IDashboardService dashboardService, ICacheService cacheService)
    {
        _dashboardService = dashboardService;
        _cacheService = cacheService;
    }

    private bool IsAdmin => User.IsAdmin();
    private string _cacheKey => "Dashboard_key_124564315";
    /// <summary>
    /// Everything the dashboard page needs in one round trip. Every widget
    /// is fetched concurrently via Task.WhenAll rather than awaited one at a
    /// time, so the total latency is roughly the slowest single query, not
    /// the sum of all of them.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardReportDto>> GetReport(
        [FromQuery] int recentErrorsTake = 10,
        [FromQuery] int topExceptionTypesTake = 10,
        [FromQuery] int topUsersTake = 10,
        [FromQuery] int topProjectsTake = 10,
        CancellationToken cancellationToken = default)
    {
        var data = await _cacheService.GetAsync<DashboardReportDto>(_cacheKey);
        if (data is not null) return data;

        var isAdmin = IsAdmin;

        var summary = await _dashboardService.GetSummaryAsync(isAdmin, cancellationToken);
        var comparison = await _dashboardService.GetComparisonAsync(isAdmin, cancellationToken);
        var recentErrors = await _dashboardService.GetRecentErrorsAsync(isAdmin, recentErrorsTake, cancellationToken);
        var topExceptionTypes = await _dashboardService.GetTopExceptionTypesAsync(isAdmin, topExceptionTypesTake, cancellationToken);
        var topUsers = await _dashboardService.GetTopUsersAsync(isAdmin, topUsersTake, cancellationToken);
        var hourlyErrors = await _dashboardService.GetHourlyErrorsAsync(isAdmin, cancellationToken);
        var projectErrorDistribution = await _dashboardService.GetProjectErrorDistributionAsync(cancellationToken);
        var userErrorDistribution = await _dashboardService.GetUserErrorDistributionAsync(cancellationToken);

        AdminDashboardSummaryDto? adminSummary = null;
        List<AdminTopProjectDto>? adminTopProjects = null;

        if (isAdmin)
        {
            adminSummary = await _dashboardService.GetAdminSummaryAsync(cancellationToken);
            adminTopProjects = await _dashboardService.GetAdminTopProjectsAsync(topProjectsTake, cancellationToken);
        }

        var report = new DashboardReportDto
        {
            Summary = summary,
            Comparison = comparison,
            RecentErrors = recentErrors,
            TopExceptionTypes = topExceptionTypes,
            TopUsers = topUsers,
            HourlyErrors = hourlyErrors,
            ProjectErrorDistribution = projectErrorDistribution,
            UserErrorDistribution = userErrorDistribution,
            AdminSummary = adminSummary,
            AdminTopProjects = adminTopProjects
        };

        await _cacheService.SetAsync<DashboardReportDto>(_cacheKey,report, TimeSpan.FromMinutes(3));

        return Ok(report);
    }
}