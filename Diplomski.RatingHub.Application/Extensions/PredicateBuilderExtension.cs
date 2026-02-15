using System.Linq.Expressions;
using Diplomski.RatingHub.Application.Utilities;

namespace Diplomski.RatingHub.Application.Extensions;

public static class PredicateBuilderExtension
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
    {
        ParameterExpression p = a.Parameters[0];

        SubstExpressionVisitor visitor = new SubstExpressionVisitor { Subst = { [b.Parameters[0]] = p } };

        Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }
}