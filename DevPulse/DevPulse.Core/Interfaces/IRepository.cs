using Ardalis.Specification;

namespace DevPulse.Infrastructure.Repository.Contracts;

public interface IRepository<T> : IRepositoryBase<T> , IReadRepositoryBase<T> where T : class
{
}