using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Projects;
using DevPulse.Core.Entities.Projects;

namespace DevPulse.Application.Services.Projects.Inetfaces;

public interface IUserService
{
    Task<UserDetailDto?> GetByIdAsync(Guid id);

    Task<PagedResult<UserListItemDto>> GetUsersAsync(PaginationRequest request);

    Task<User> CreateUserAsync(CreateUserRequest request);
}
