using System.Linq.Expressions;
using Diplomski.RatingHub.Application.Extensions;
using Diplomski.RatingHub.Application.Interfaces.Specifications;
using Diplomski.RatingHub.Application.Models;

namespace Diplomski.RatingHub.Application.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    public virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    public virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    public virtual void ApplyPaging(QueryArgs args)
    {
        if (args.Skip == null || args.Take == null)
        {
            return;
        }

        Skip = args.Skip.Value;
        Take = args.Take.Value;
        IsPagingEnabled = true;
    }

    public virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    public virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    public void And(Expression<Func<T, bool>> criteria)
    {
        Criteria = Criteria.And(criteria);
    }
}