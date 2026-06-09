using Diplomski.RatingHub.Application.Interfaces.Specifications;
using Diplomski.RatingHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Diplomski.RatingHub.Infrastructure.Persistence.Repositories.Utilities;

public class SpecificationEvaluator<T> where T : class, IDatabaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        IQueryable<T> query = inputQuery;

        // modify the IQueryable using the specification's criteria expression
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Includes all expression-based includes
        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        // Include any string-based include statements
        query = specification.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));

        // Apply ordering if expressions are set
        if (specification.OrderBy != null)
        {
            if(specification.ThenOrderBy != null)
                query = query.OrderBy(specification.OrderBy).ThenBy(specification.ThenOrderBy);
            else if(specification.ThenOrderByDescending != null)
                query = query.OrderBy(specification.OrderBy).ThenByDescending(specification.ThenOrderByDescending);
            else
                query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            if (specification.ThenOrderBy != null)
                query = query.OrderByDescending(specification.OrderByDescending).ThenBy(specification.ThenOrderBy);
            else if(specification.ThenOrderByDescending != null)
                query = query.OrderByDescending(specification.OrderByDescending).ThenByDescending(specification.ThenOrderByDescending);
            else
                query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Apply paging if enabled
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip)
                .Take(specification.Take);
        }
            
        return query;
    }
}