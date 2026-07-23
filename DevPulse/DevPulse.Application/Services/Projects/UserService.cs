using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Projects;
using DevPulse.Application.Services.Projects.Inetfaces;
using DevPulse.Application.Specifications.Users;
using DevPulse.Core.Entities.Projects;
using DevPulse.Core.Exceptions;
using DevPulse.Infrastructure.Repository.Contracts;
using Microsoft.AspNetCore.Identity;

namespace DevPulse.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepo;
    private readonly IProjectService _projectService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IRepository<User> userRepo, IProjectService projectService, IPasswordHasher<User> passwordHasher)
    {
        _userRepo = userRepo;
        _projectService = projectService;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        var user = new User(request.Email, request.FullName, false);

        user.ChangePassword(_passwordHasher.HashPassword(user, request.Password));

        await _userRepo.AddAsync(user);

        await _projectService.CreateAsync(user.Id, $"پروژه {request.FullName}!");

        return user;
    }

    public async Task<UserDetailDto?> GetByIdAsync(Guid id)
    {
        var spec = new UserByIdSpec(id);

        var user = await _userRepo.FirstOrDefaultAsync(spec);

        if (user is null)
            throw new NotFoundException(ErrorMessages.UserNotFound , "USER_NOT_FOUND");

        return new UserDetailDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            IsAdmin = user.IsAdmin,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,

            Projects = user.Projects.Select(p => new ProjectListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                ApiKey = p.ApiKey,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            }).ToList()
        };
    }

    public async Task<PagedResult<UserListItemDto>> GetUsersAsync(PaginationRequest request)
    {
        var spec = new UsersSpec(request);

        var countSpec = new UsersCountSpec();

        var users = await _userRepo.ListAsync(spec);

        return new PagedResult<UserListItemDto>
        {
            Items = users.Select(x => new UserListItemDto
            {
                Id = x.Id,
                Email = x.Email,
                FullName = x.FullName,
                IsAdmin = x.IsAdmin,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                LastLoginAt = x.LastLoginAt,
                ProjectCount = x.Projects.Count
            }).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = await _userRepo.CountAsync(countSpec)
        };
    }
}