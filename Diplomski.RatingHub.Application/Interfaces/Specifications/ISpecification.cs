using System.Linq.Expressions;
using Diplomski.RatingHub.Application.Models;

namespace Diplomski.RatingHub.Application.Interfaces.Specifications;

public interface  ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; set; }
    void ApplyOrderBy(Expression<Func<T, object>> orderByExpression);
    void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression);
    void ApplyPaging(int skip, int take);
    void ApplyPaging(QueryArgs args);
    void AddInclude(Expression<Func<T, object>> includeExpression);
    void AddInclude(string includeString);
    void And(Expression<Func<T, bool>> criteria);
}