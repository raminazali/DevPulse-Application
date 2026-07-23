using Ardalis.Specification;
using DevPulse.Application.Common;
using DevPulse.Core.Entities.Projects;

namespace DevPulse.Application.Specifications.Users;

public class UsersSpec : Specification<User>
{
    public UsersSpec(PaginationRequest pagination)
    {
        Query
            .Include(x => x.Projects) 
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedAt);

        if (pagination.PageSize > 0)
            Query.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize);
    }
}

public class UsersCountSpec:  Specification<User>
{
    public UsersCountSpec()
    {
        Query.Where(x => x.IsActive);
    }
}