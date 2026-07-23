using DevPulse.Application.DTOs.Dashboards;
using DevPulse.Application.Services.Dashboard.Interfaces;
using DevPulse.Application.Specifications.Dashboard;
using DevPulse.Application.Specifications.Projects;
using DevPulse.Application.Specifications.Users;
using DevPulse.Core.Entities.Errors;
using DevPulse.Core.Entities.Projects;
using DevPulse.Infrastructure.Repository.Contracts;

namespace DevPulse.Application.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IRepository<ErrorLog> _errorLogRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<User> _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public DashboardService(
        IRepository<ErrorLog> errorLogRepository,
        IRepository<Project> projectRepository,
        IRepository<User> userRepository,
        ICurrentUserService currentUserService)
    {
        _errorLogRepository = errorLogRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    private Guid? GetOwnerId(bool isAdmin) => isAdmin ? null : _currentUserService.UserId;

    public async Task<AdminDashboardSummaryDto> GetAdminSummaryAsync(CancellationToken cancellationToken)
    {
        var totalUsers = await _userRepository.CountAsync(cancellationToken);
        var activeUsers = await _userRepository.CountAsync(new ActiveUsersSpecification(), cancellationToken);

        var totalProjects = await _projectRepository.CountAsync(cancellationToken);
        var activeProjects = await _projectRepository.CountAsync(
            new ActiveProjectsSpecification(ownerId: null, isAdmin: true), cancellationToken);

        var totalErrors = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(DateTime.MinValue, null, null, isAdmin: true), cancellationToken);
        var todayErrors = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(DateTime.UtcNow.Date, null, null, isAdmin: true), cancellationToken);

        return new AdminDashboardSummaryDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            TotalProjects = totalProjects,
            ActiveProjects = activeProjects,
            TotalErrors = totalErrors,
            TodayErrors = todayErrors
        };
    }

    public async Task<List<AdminTopProjectDto>> GetAdminTopProjectsAsync(int take, CancellationToken cancellationToken)
    {
        var logs = await _errorLogRepository.ListAsync(
            new TopProjectsSpecification(isAdmin: true, ownerId: null), cancellationToken);

        return logs
            .GroupBy(x => x.ProjectId)
            .Select(g => new AdminTopProjectDto
            {
                ProjectId = g.Key,
                ProjectName = g.First().Project.Name,
                OwnerName = g.First().Project.User.FullName,
                ErrorCount = g.LongCount()
            })
            .OrderByDescending(x => x.ErrorCount)
            .Take(take)
            .ToList();
    }

    public async Task<ErrorComparisonDto> GetComparisonAsync(bool isAdmin, CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerId(isAdmin);
        var today = DateTime.UtcNow.Date;

        var todayCount = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(today, today.AddDays(1), ownerId, isAdmin), cancellationToken);
        var yesterdayCount = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(today.AddDays(-1), today, ownerId, isAdmin), cancellationToken);

        var weekCount = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(today.AddDays(-7), today.AddDays(1), ownerId, isAdmin), cancellationToken);
        var lastWeekCount = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(today.AddDays(-14), today.AddDays(-7), ownerId, isAdmin), cancellationToken);

        var monthCount = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(today.AddDays(-30), today.AddDays(1), ownerId, isAdmin), cancellationToken);
        var lastMonthCount = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(today.AddDays(-60), today.AddDays(-30), ownerId, isAdmin), cancellationToken);

        return new ErrorComparisonDto
        {
            Today = todayCount,
            Yesterday = yesterdayCount,
            TodayPercentage = CalculatePercentageChange(todayCount, yesterdayCount),

            Week = weekCount,
            LastWeek = lastWeekCount,
            WeekPercentage = CalculatePercentageChange(weekCount, lastWeekCount),

            Month = monthCount,
            LastMonth = lastMonthCount,
            MonthPercentage = CalculatePercentageChange(monthCount, lastMonthCount)
        };
    }

    private static double CalculatePercentageChange(long current, long previous)
    {
        if (previous == 0)
        {
            return current == 0 ? 0 : 100;
        }

        return Math.Round((current - previous) / (double)previous * 100, 2);
    }

    public async Task<List<KeyValueDto>> GetHourlyErrorsAsync(bool isAdmin, CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerId(isAdmin);
        var logs = await _errorLogRepository.ListAsync(
            new HourlyErrorsSpecification(ownerId, isAdmin), cancellationToken);

        var today = DateTime.UtcNow.Date;
        var byHour = logs
            .Where(x => x.CreatedAt.Date == today)
            .GroupBy(x => x.CreatedAt.Hour)
            .ToDictionary(g => g.Key, g => (long)g.Count());

        return Enumerable.Range(0, 24)
            .Select(hour => new KeyValueDto
            {
                Label = $"{hour:D2}:00",
                Value = byHour.TryGetValue(hour, out var count) ? count : 0
            })
            .ToList();
    }

    public async Task<List<KeyValueDto>> GetProjectErrorDistributionAsync(CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.UserId;
        var logs = await _errorLogRepository.ListAsync(
            new TopProjectsSpecification(isAdmin: false, ownerId: ownerId), cancellationToken);

        return logs
            .GroupBy(x => x.ProjectId)
            .Select(g => new KeyValueDto { Label = g.First().Project.Name, Value = g.LongCount() })
            .OrderByDescending(x => x.Value)
            .ToList();
    }

    public async Task<List<KeyValueDto>> GetUserErrorDistributionAsync(CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.UserId;
        var logs = await _errorLogRepository.ListAsync(
            new DashboardSummarySpecification(ownerId, isAdmin: false), cancellationToken);

        return logs
            .GroupBy(x => x.UserId)
            .Select(g => new KeyValueDto
            {
                Label = g.Key.HasValue ? g.Key.Value.ToString() : "Anonymous",
                Value = g.LongCount()
            })
            .OrderByDescending(x => x.Value)
            .ToList();
    }

    public async Task<List<RecentErrorDto>> GetRecentErrorsAsync(bool isAdmin, int take, CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerId(isAdmin);
        var logs = await _errorLogRepository.ListAsync(
            new RecentErrorsSpecification(isAdmin, ownerId, take), cancellationToken);

        return logs
            .Select(x => new RecentErrorDto
            {
                ErrorId = x.Id,
                ProjectId = x.ProjectId,
                ProjectName = x.Project.Name,
                ExceptionType = x.ExceptionType,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(bool isAdmin, CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerId(isAdmin);
        var today = DateTime.UtcNow.Date;

        var totalErrors = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(DateTime.MinValue, null, ownerId, isAdmin), cancellationToken);
        var todayErrors = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(today, null, ownerId, isAdmin), cancellationToken);
        var weekErrors = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(today.AddDays(-7), null, ownerId, isAdmin), cancellationToken);
        var monthErrors = await _errorLogRepository.CountAsync(
            new ErrorsInRangeSpecification(today.AddDays(-30), null, ownerId, isAdmin), cancellationToken);

        var activeProjects = await _projectRepository.CountAsync(
            new ActiveProjectsSpecification(ownerId, isAdmin), cancellationToken);

        var mostRecent = await _errorLogRepository.ListAsync(
            new RecentErrorsSpecification(isAdmin, ownerId, 1), cancellationToken);

        return new DashboardSummaryDto
        {
            TotalErrors = totalErrors,
            TodayErrors = todayErrors,
            WeekErrors = weekErrors,
            MonthErrors = monthErrors,
            ActiveProjects = activeProjects,
            LastErrorAt = mostRecent.Count > 0 ? mostRecent[0].CreatedAt : null
        };
    }

    public async Task<List<KeyValueDto>> GetTopExceptionTypesAsync(bool isAdmin, int take, CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerId(isAdmin);
        var logs = await _errorLogRepository.ListAsync(
            new TopExceptionTypesSpecification(ownerId, isAdmin), cancellationToken);

        return logs
            .GroupBy(x => x.ExceptionType)
            .Select(g => new KeyValueDto { Label = g.Key, Value = g.LongCount() })
            .OrderByDescending(x => x.Value)
            .Take(take)
            .ToList();
    }

    public async Task<List<TopUserDto>> GetTopUsersAsync(bool isAdmin, int take, CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerId(isAdmin);
        var logs = await _errorLogRepository.ListAsync(
            new TopUsersSpecification(ownerId, isAdmin), cancellationToken);

        return logs
            .GroupBy(x => x.Project.UserId)
            .Select(g => new TopUserDto
            {
                UserId = g.Key,
                FullName = g.First().Project.User.FullName,
                ProjectCount = g.Select(x => x.ProjectId).Distinct().Count(),
                ErrorCount = g.LongCount()
            })
            .OrderByDescending(x => x.ErrorCount)
            .Take(take)
            .ToList();
    }
}