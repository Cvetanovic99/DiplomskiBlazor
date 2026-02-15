using System.Linq.Expressions;

namespace Diplomski.RatingHub.Application.Utilities;

public class SubstExpressionVisitor : ExpressionVisitor
{
    public readonly Dictionary<Expression, Expression> Subst = new();

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return Subst.TryGetValue(node, out Expression newValue) ? newValue : node;
    }
}