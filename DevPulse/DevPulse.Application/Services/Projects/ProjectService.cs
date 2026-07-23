using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Projects;
using DevPulse.Application.Services.Projects.Inetfaces;
using DevPulse.Application.Specifications.Projects;
using DevPulse.Core.Entities.Projects;
using DevPulse.Core.Exceptions;
using DevPulse.Infrastructure.Repository.Contracts;

namespace DevPulse.Application.Services.Projects;

public class ProjectService : IProjectService
{
    private readonly IRepository<Project> _repository;

    public ProjectService(IRepository<Project> repository)
    {
        _repository = repository;
    }

    public async Task<Project> CreateAsync(Guid userId, string name)
    {
        var project = new Project(name, userId);

        await _repository.AddAsync(project);

        return project;
    }
    public async Task<Project?> GetByIdEntityAsync(Guid id, Guid userId, bool isAdmin)
    {
        var spec = new ProjectByIdSpec(id, userId, isAdmin);

        return await _repository.FirstOrDefaultAsync(spec);
    }
    public async Task<ProjectDetailDto?> GetByIdAsync(Guid id, Guid userId, bool isAdmin)
    {
        var project = await GetByIdEntityAsync(id, userId, isAdmin);

        if (project is null)
            throw new NotFoundException($"Project with ID '{id}' was not found.", "PROJECT_NOT_FOUND"); ;

        return new ProjectDetailDto
        {
            Id = project.Id,
            Name = project.Name,
            ApiKey = project.ApiKey,
            IsActive = project.IsActive,
            CreatedAt = project.CreatedAt,
            UserId = project.UserId,
            UserEmail = project.User.Email
        };
    }

    public async Task<PagedResult<ProjectListItemDto>> GetUserProjectsAsync(Guid? userId, bool isAdmin, PaginationRequest request)
    {
        var specCount = new ProjectCountSpec(userId, isAdmin);
        var spec = new ProjectSpec(userId, isAdmin, request);

        var projects = await _repository.ListAsync(spec);

        return new PagedResult<ProjectListItemDto>
        {
            Items = projects.Select(x => new ProjectListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                ApiKey = x.ApiKey,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                ErrorCount = x.ErrorLogs.Count()
            }).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = await _repository.CountAsync(specCount)
        };
    }

    public async Task UpdateAsync(UpdateProjectRequest request, Guid userId, bool isAdmin)
    {
        var project = await GetByIdEntityAsync(request.Id, userId, isAdmin);

        if (project is null)
            throw new NotFoundException($"پروژه با شناسه '{request.Id}' یافت نشد.", "PROJECT_NOT_FOUND");

        project.Update(request.Name, request.IsActive);

        await _repository.UpdateAsync(project);
    }

    public async Task DeleteAsync(Guid id, Guid userId, bool isAdmin)
    {
        var project = await GetByIdEntityAsync(id, userId, isAdmin);

        if (project is null)
            throw new NotFoundException($"پروژه با شناسه '{id}' یافت نشد.", "PROJECT_NOT_FOUND");

        await _repository.DeleteAsync(project);
    }
}