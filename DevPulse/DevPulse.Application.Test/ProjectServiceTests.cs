using Ardalis.Specification;
using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Projects;
using DevPulse.Application.Services.Projects;
using DevPulse.Application.Services.Projects.Inetfaces;
using DevPulse.Application.Specifications.Projects;
using DevPulse.Core.Entities.Projects;
using DevPulse.Core.Exceptions;
using DevPulse.Infrastructure.Repository.Contracts;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DevPulse.Application.Tests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IRepository<Project>> _projectRepoMock;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _projectRepoMock = new Mock<IRepository<Project>>();
            _projectService = new ProjectService(_projectRepoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedProject()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var name = "New Project";
            var projectId = Guid.NewGuid();

            _projectRepoMock
                .Setup(r => r.AddAsync(
                    It.IsAny<Project>(),
                    It.IsAny<CancellationToken>()))
                .Callback<Project, CancellationToken>((p, _) =>
                {
                    p.Id = projectId;
                })
                .ReturnsAsync((Project p, CancellationToken _) => p);

            // Act
            var result = await _projectService.CreateAsync(ownerId, name);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectId, result.Id);
            Assert.Equal(ownerId, result.UserId);
            Assert.Equal(name, result.Name);
            Assert.False(string.IsNullOrWhiteSpace(result.ApiKey));
            Assert.True(result.IsActive);

            _projectRepoMock.Verify(r => r.AddAsync(
                It.Is<Project>(p =>
                    p.Id == projectId &&
                    p.UserId == ownerId &&
                    p.Name == name &&
                    !string.IsNullOrWhiteSpace(p.ApiKey) &&
                    p.IsActive),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsProjectDetailDto_WhenProjectExists()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var project = new Project("Test Project", ownerId)
            {
                Id = projectId
            };

            var user = new User("owner@test.com", "Owner Name", false);
            typeof(Project).GetProperty("User")?.SetValue(project, user);

            _projectRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProjectByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            // Act
            var result = await _projectService.GetByIdAsync(projectId, ownerId, isAdmin: false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectId, result.Id);
            Assert.Equal(project.Name, result.Name);
            Assert.Equal(project.ApiKey, result.ApiKey);
            Assert.Equal(project.IsActive, result.IsActive);
            Assert.Equal(project.CreatedAt.Date, result.CreatedAt.Date);
            Assert.Equal(ownerId, result.UserId);
            Assert.Equal("owner@test.com", result.UserEmail);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsNotFoundException_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _projectRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProjectByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Project?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _projectService.GetByIdAsync(projectId, userId, isAdmin: false));

            Assert.Equal("PROJECT_NOT_FOUND", exception.Code);
        }

        [Fact]
        public async Task GetUserProjectsAsync_ReturnsPagedResult_WithCorrectData()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var otherId = Guid.NewGuid();

            var projects = new List<Project>
            {
                new Project("Project 1", ownerId) { Id = Guid.NewGuid() },
                new Project("Project 2", ownerId) { Id = Guid.NewGuid() },
                new Project("Project 3", ownerId) { Id = Guid.NewGuid() },
                new Project("Project 4", otherId) { Id = Guid.NewGuid() }
            };

            var page = 1;
            var pageSize = 2;
            var request = new PaginationRequest { Page = page, PageSize = pageSize };

            _projectRepoMock.Setup(r => r.ListAsync(It.IsAny<ProjectSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(projects.Take(pageSize).ToList());

            _projectRepoMock.Setup(r => r.CountAsync(It.IsAny<ProjectCountSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(projects.Count);

            // Act
            var result = await _projectService.GetUserProjectsAsync(ownerId, true, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(page, result.Page);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(projects.Count, result.TotalCount);

            Assert.Equal(2, result.Items.Count);
            Assert.Contains(result.Items, i => i.Name == "Project 1");
            Assert.Contains(result.Items, i => i.Name == "Project 2");

            foreach (var item in result.Items)
            {
                Assert.Contains(projects, p => p.Id == item.Id && p.Name == item.Name);
            }
        }

        [Fact]
        public async Task UpdateAsync_UpdatesProjectAndReturnsNothing()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var existingProject = new Project("Old Name", ownerId)
            {
                Id = projectId
            };

            var updateRequest = new UpdateProjectRequest
            {
                Id = projectId,
                Name = "New Name",
                IsActive = false
            };

            _projectRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProjectByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProject);

            _projectRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _projectService.UpdateAsync(updateRequest, ownerId, isAdmin: false);

            // Assert
            Assert.Equal("New Name", existingProject.Name);
            Assert.False(existingProject.IsActive);
            _projectRepoMock.Verify(r => r.UpdateAsync(It.Is<Project>(p =>
                p.Id == projectId &&
                p.Name == "New Name" &&
                p.IsActive == false), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_RemovesProject_WhenExists()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var project = new Project("To Delete", ownerId)
            {
                Id = projectId
            };

            _projectRepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<ProjectByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            _projectRepoMock.Setup(r => r.DeleteAsync(
                    It.IsAny<Project>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _projectService.DeleteAsync(projectId, ownerId, isAdmin: false);

            // Assert
            _projectRepoMock.Verify(r => r.DeleteAsync(
                It.Is<Project>(p => p.Id == projectId && p.Name == "To Delete"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsNotFoundException_WhenProjectNotFound()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            _projectRepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<ProjectByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Project?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _projectService.DeleteAsync(projectId, ownerId, isAdmin: false));

            Assert.Equal("PROJECT_NOT_FOUND", exception.Code);
            _projectRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
