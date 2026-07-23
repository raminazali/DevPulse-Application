using Ardalis.Specification;
using DevPulse.Core.Entities.Projects;

namespace DevPulse.Application.Specifications.Users;

public class UserByEmailSpec : Specification<User>
{
    public UserByEmailSpec(string email)
    {
        Query
            .Where(x => x.Email == email.ToLowerInvariant() && x.IsActive)
            .Include(x => x.Projects);
    }
}