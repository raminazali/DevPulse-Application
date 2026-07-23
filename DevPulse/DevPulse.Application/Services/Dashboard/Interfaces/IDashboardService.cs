using DevPulse.Application.DTOs.Dashboards;

namespace DevPulse.Application.Services.Dashboard.Interfaces;

public interface IDashboardService
{
    // ==========================
    // Common (Admin & User)
    // ==========================

    Task<DashboardSummaryDto> GetSummaryAsync(bool isAdmin, CancellationToken cancellationToken);

    Task<List<KeyValueDto>> GetTopExceptionTypesAsync(
        bool isAdmin,
        int take,
        CancellationToken cancellationToken);

    Task<List<KeyValueDto>> GetHourlyErrorsAsync(
        bool isAdmin,
        CancellationToken cancellationToken);

    Task<ErrorComparisonDto> GetComparisonAsync(
        bool isAdmin,
        CancellationToken cancellationToken);

    Task<List<RecentErrorDto>> GetRecentErrorsAsync(
        bool isAdmin,
        int take,
        CancellationToken cancellationToken);



    // ==========================
    // Admin Only
    // ==========================

    Task<AdminDashboardSummaryDto> GetAdminSummaryAsync(
        CancellationToken cancellationToken);

    Task<List<TopUserDto>> GetTopUsersAsync(bool isAdmin, int take, CancellationToken cancellationToken);


    Task<List<AdminTopProjectDto>> GetAdminTopProjectsAsync(
        int take,
        CancellationToken cancellationToken);

    Task<List<KeyValueDto>> GetUserErrorDistributionAsync(
        CancellationToken cancellationToken);

    Task<List<KeyValueDto>> GetProjectErrorDistributionAsync(
        CancellationToken cancellationToken);
}
