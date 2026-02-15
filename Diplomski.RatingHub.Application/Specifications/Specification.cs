using System.Linq.Expressions;
using Diplomski.RatingHub.Application.Models;

namespace Diplomski.RatingHub.Application.Specifications;

public sealed class Specification<T> : BaseSpecification<T>
{
    public Specification(Expression<Func<T, bool>> criteria) : base(criteria)
    {
    }
    
    public new Specification<T> AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
        return this;
    }
    
    public new Specification<T> ApplyPaging(int skip, int take)
    {
        base.ApplyPaging(skip, take);
        return this;
    }

    public new Specification<T> AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
        return this;
    }
    
    public new Specification<T> ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        base.ApplyOrderBy(orderByExpression);
        return this;
    }

    public new Specification<T> ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        base.ApplyOrderByDescending(orderByDescendingExpression);
        return this;
    }
    
    public new Specification<T> ApplyPaging(QueryArgs args)
    {
        base.ApplyPaging(args);
        return this;
    }
    
    public new Specification<T> And(Expression<Func<T, bool>> criteria)
    {
        base.And(criteria);
        return this;
    }
}