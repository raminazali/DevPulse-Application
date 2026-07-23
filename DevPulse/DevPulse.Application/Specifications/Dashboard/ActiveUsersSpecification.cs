using Ardalis.Specification;
using DevPulse.Core.Entities.Projects;

namespace DevPulse.Application.Specifications.Users;

public sealed class ActiveUsersSpecification : Specification<User>
{
    public ActiveUsersSpecification()
    {
        Query.AsNoTracking().Where(x => x.IsActive);
    }
}