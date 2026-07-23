using DevPulse.Application.DTOs.Dashboards;
using DevPulse.Application.Services.Dashboard;
using DevPulse.Application.Services.Dashboard.Interfaces;
using DevPulse.Application.Specifications.Dashboard;
using DevPulse.Application.Specifications.Projects;
using DevPulse.Application.Specifications.Users;
using DevPulse.Core.Entities.Errors;
using DevPulse.Core.Entities.Projects;
using DevPulse.Infrastructure.Repository.Contracts;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DevPulse.Application.Tests.Services
{
    public class DashboardServiceTests
    {
        private readonly Mock<IRepository<ErrorLog>> _errorLogRepoMock;
        private readonly Mock<IRepository<Project>> _projectRepoMock;
        private readonly Mock<IRepository<User>> _userRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly DashboardService _dashboardService;

        private readonly Guid _testUserId = Guid.NewGuid();
        private readonly Guid _testProjectId = Guid.NewGuid();

        public DashboardServiceTests()
        {
            _errorLogRepoMock = new Mock<IRepository<ErrorLog>>();
            _projectRepoMock = new Mock<IRepository<Project>>();
            _userRepoMock = new Mock<IRepository<User>>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _currentUserServiceMock.Setup(x => x.UserId).Returns(_testUserId);

            _dashboardService = new DashboardService(
                _errorLogRepoMock.Object,
                _projectRepoMock.Object,
                _userRepoMock.Object,
                _currentUserServiceMock.Object);
        }

        private ErrorLog CreateTestErrorLog(Guid projectId, DateTime createdAt, string projectName = "Test Project", string userName = "Test User")
        {
            var errorLog = ErrorLog.Create(
                projectId: projectId,
                message: "Test Error",
                stackTrace: "stack",
                url: "http://test.com",
                exceptionType: "InvalidOperationException",
                method: "GET",
                requestBody: "{}",
                queryString: "id=1",
                userId: _testUserId,
                browser: "Chrome",
                ipAddress: "127.0.0.1",
                createdAt: createdAt
            );

            var project = new Project(projectName, _testUserId);
            typeof(Project).GetProperty("Id")?.SetValue(project, projectId);

            var user = new User("test@test.com", userName, false);
            typeof(User).GetProperty("Id")?.SetValue(user, _testUserId);
            typeof(Project).GetProperty("User")?.SetValue(project, user);

            typeof(ErrorLog).GetProperty("Project")?.SetValue(errorLog, project);

            return errorLog;
        }

        [Fact]
        public async Task GetAdminSummaryAsync_ReturnsCorrectCounts()
        {
            // Arrange
            _userRepoMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(10);
            _userRepoMock.Setup(r => r.CountAsync(It.IsAny<ActiveUsersSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(8);
            _projectRepoMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);
            _projectRepoMock.Setup(r => r.CountAsync(It.IsAny<ActiveProjectsSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(4);
            _errorLogRepoMock.SetupSequence(r => r.CountAsync(It.IsAny<ErrorsInRangeSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(100)
                .ReturnsAsync(10);

            // Act
            var result = await _dashboardService.GetAdminSummaryAsync(CancellationToken.None);

            // Assert
            Assert.Equal(10, result.TotalUsers);
            Assert.Equal(8, result.ActiveUsers);
            Assert.Equal(5, result.TotalProjects);
            Assert.Equal(4, result.ActiveProjects);
            Assert.Equal(100, result.TotalErrors);
            Assert.Equal(10, result.TodayErrors);
        }

        [Fact]
        public async Task GetAdminTopProjectsAsync_ReturnsGroupedAndOrderedProjects()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var logs = new List<ErrorLog>
            {
                CreateTestErrorLog(_testProjectId, now, "Project A", "Admin User"),
                CreateTestErrorLog(_testProjectId, now, "Project A", "Admin User"),
                CreateTestErrorLog(Guid.NewGuid(), now, "Project B", "Admin User")
            };

            _errorLogRepoMock.Setup(r => r.ListAsync(It.IsAny<TopProjectsSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(logs);

            // Act
            var result = await _dashboardService.GetAdminTopProjectsAsync(1, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal(_testProjectId, result[0].ProjectId);
            Assert.Equal("Project A", result[0].ProjectName);
            Assert.Equal("Admin User", result[0].OwnerName);
            Assert.Equal(2, result[0].ErrorCount);
        }

        [Fact]
        public async Task GetComparisonAsync_CalculatesPercentagesCorrectly()
        {
            // Arrange
            _errorLogRepoMock.SetupSequence(r => r.CountAsync(It.IsAny<ErrorsInRangeSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(10)   // Today
                .ReturnsAsync(5)    // Yesterday
                .ReturnsAsync(20)   // Week
                .ReturnsAsync(20)   // LastWeek
                .ReturnsAsync(0)    // Month
                .ReturnsAsync(0);   // LastMonth

            // Act
            var result = await _dashboardService.GetComparisonAsync(isAdmin: true, CancellationToken.None);

            // Assert
            Assert.Equal(10, result.Today);
            Assert.Equal(5, result.Yesterday);
            Assert.Equal(100.0, result.TodayPercentage);

            Assert.Equal(20, result.Week);
            Assert.Equal(20, result.LastWeek);
            Assert.Equal(0.0, result.WeekPercentage);

            Assert.Equal(0, result.Month);
            Assert.Equal(0, result.LastMonth);
            Assert.Equal(0.0, result.MonthPercentage);
        }

        [Fact]
        public async Task GetHourlyErrorsAsync_Returns24HoursWithZeroFallback()
        {
            // Arrange
            var today = DateTime.UtcNow.Date;
            var logs = new List<ErrorLog>
            {
                CreateTestErrorLog(_testProjectId, today.AddHours(5)),
                CreateTestErrorLog(_testProjectId, today.AddHours(5)),
                CreateTestErrorLog(_testProjectId, today.AddHours(14))
            };

            _errorLogRepoMock.Setup(r => r.ListAsync(It.IsAny<HourlyErrorsSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(logs);

            // Act
            var result = await _dashboardService.GetHourlyErrorsAsync(isAdmin: true, CancellationToken.None);

            // Assert
            Assert.Equal(24, result.Count);
            Assert.Equal("05:00", result[5].Label);
            Assert.Equal(2, result[5].Value);
            Assert.Equal("14:00", result[14].Label);
            Assert.Equal(1, result[14].Value);
            Assert.Equal(0, result[0].Value);
        }

        [Fact]
        public async Task GetProjectErrorDistributionAsync_GroupsErrorsByProject()
        {
            // Arrange
            var logs = new List<ErrorLog>
            {
                CreateTestErrorLog(_testProjectId, DateTime.UtcNow, "Project Alpha"),
                CreateTestErrorLog(_testProjectId, DateTime.UtcNow, "Project Alpha"),
                CreateTestErrorLog(Guid.NewGuid(), DateTime.UtcNow, "Project Beta")
            };

            _errorLogRepoMock.Setup(r => r.ListAsync(It.IsAny<TopProjectsSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(logs);

            // Act
            var result = await _dashboardService.GetProjectErrorDistributionAsync(CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Project Alpha", result[0].Label);
            Assert.Equal(2, result[0].Value);
            Assert.Equal("Project Beta", result[1].Label);
            Assert.Equal(1, result[1].Value);
        }

        [Fact]
        public async Task GetRecentErrorsAsync_MapsToDtoCorrectly()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var logs = new List<ErrorLog>
            {
                CreateTestErrorLog(_testProjectId, now, "Recent Project")
            };

            _errorLogRepoMock.Setup(r => r.ListAsync(It.IsAny<RecentErrorsSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(logs);

            // Act
            var result = await _dashboardService.GetRecentErrorsAsync(isAdmin: true, take: 5, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal("Recent Project", result[0].ProjectName);
            Assert.Equal("InvalidOperationException", result[0].ExceptionType);
            Assert.Equal(now, result[0].CreatedAt);
        }

        [Fact]
        public async Task GetTopExceptionTypesAsync_ReturnsTopTypes()
        {
            // Arrange
            var logs = new List<ErrorLog>
            {
                CreateTestErrorLog(_testProjectId, DateTime.UtcNow),
                CreateTestErrorLog(_testProjectId, DateTime.UtcNow),
                CreateTestErrorLog(Guid.NewGuid(), DateTime.UtcNow)
            };
            typeof(ErrorLog).GetProperty("ExceptionType")?.SetValue(logs[2], "NullReferenceException");

            _errorLogRepoMock.Setup(r => r.ListAsync(It.IsAny<TopExceptionTypesSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(logs);

            // Act
            var result = await _dashboardService.GetTopExceptionTypesAsync(isAdmin: true, take: 1, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal("InvalidOperationException", result[0].Label);
            Assert.Equal(2, result[0].Value);
        }
    }
}
