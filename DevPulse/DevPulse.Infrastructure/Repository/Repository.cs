using Ardalis.Specification.EntityFrameworkCore;
using DevPulse.Infrastructure.Context;
using DevPulse.Infrastructure.Repository.Contracts;

namespace DevPulse.Infrastructure.Repository;

public class Repository<T> : RepositoryBase<T>, IRepository<T> where T : class
{
    public Repository(DevPulseDbContext dbContext) : base(dbContext)
    {
    }

}
