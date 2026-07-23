using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Projects;
using DevPulse.Core.Entities.Projects;

namespace DevPulse.Application.Services.Projects.Inetfaces;

public interface IProjectService
{
    Task<Project> CreateAsync(Guid userId, string name);

    Task<ProjectDetailDto?> GetByIdAsync(Guid id, Guid userId, bool isAdmin);

    Task<PagedResult<ProjectListItemDto>> GetUserProjectsAsync(Guid? userId , bool isAdmin, PaginationRequest request);

    Task UpdateAsync(UpdateProjectRequest request, Guid userId, bool isAdmin);

    Task DeleteAsync(Guid id, Guid userId, bool isAdmin);
}