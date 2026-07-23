using Ardalis.Specification;
using DevPulse.Core.Entities.Projects;

namespace DevPulse.Application.Specifications.Users;

public class UserByIdSpec : Specification<User>
{
    public UserByIdSpec(Guid id)
    {
        Query
            .Where(x => x.Id == id && x.IsActive)
            .Include(x => x.Projects);
    }
}