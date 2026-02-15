using Diplomski.RatingHub.Application.Interfaces.Specifications;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Domain.Interfaces;

namespace Diplomski.RatingHub.Application.Interfaces.Repositories;

public interface IRepositoryWithProjections<TEntity> where TEntity : IDatabaseEntity
{
    Task<ICollection<T>> GetAndProject<T>(ISpecification<TEntity> spec) where T : class, IMapFrom<TEntity>;
    Task<ICollection<T>> GetAllAndProject<T>() where T : class, IMapFrom<TEntity>;
    Task<T?> GetSingleAndProject<T>(ISpecification<TEntity> spec) where T : class, IMapFrom<TEntity>;
}