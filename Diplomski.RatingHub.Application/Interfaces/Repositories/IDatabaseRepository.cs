using Diplomski.RatingHub.Application.Interfaces.Specifications;
using Diplomski.RatingHub.Domain.Interfaces;

namespace Diplomski.RatingHub.Application.Interfaces.Repositories;

public interface IDatabaseRepository<T> : IRepositoryWithPaginatedList<T>, IRepositoryWithProjections<T>
where T : class, IDatabaseEntity
{
    Task<T?> GetById(int id);
    Task<List<T>> GetAll();
    Task<List<T>> Get(ISpecification<T> spec); 
    Task<T?> GetSingleBySpec(ISpecification<T> spec);
    Task<int> GetCount(ISpecification<T> spec);
    Task<T> Insert(T entity);
    Task<ICollection<T>> InsertRange(ICollection<T> entities);
    Task<T> Update(T entity);
    Task UpdateRange(IEnumerable<T> entities);
    Task Delete(T entity);
    Task DeleteRange(ICollection<T> entities);
}