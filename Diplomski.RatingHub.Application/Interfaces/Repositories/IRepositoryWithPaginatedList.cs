using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Specifications;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Domain.Interfaces;

namespace Diplomski.RatingHub.Application.Interfaces.Repositories;

public interface IRepositoryWithPaginatedList<TEntity> where TEntity : IDatabaseEntity
{
    Task<IPaginatedList<TEntity>> GetAllAsPaginatedList(QueryArgs? queryArgs = null);

    Task<IPaginatedList<TEntity>> GetAsPaginatedList(ISpecification<TEntity> spec, QueryArgs? queryArgs = null);

    Task<IPaginatedList<T>> GetAllAndProjectAsPaginatedList<T>(QueryArgs? queryArgs = null)
        where T : class, IMapFrom<TEntity>;

    Task<IPaginatedList<T>> GetAndProjectAsPaginatedList<T>(ISpecification<TEntity> spec, QueryArgs? queryArgs = null)
        where T : class, IMapFrom<TEntity>;
}