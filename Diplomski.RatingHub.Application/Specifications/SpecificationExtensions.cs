using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Specifications;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;

namespace Diplomski.RatingHub.Application.Specifications;

public static class SpecificationExtensions
{
    public static void ApplyQueryArgs<TDto, TModel>(this ISpecification<TModel> spec, IMapper mapper,
        QueryArgs? args)
        where TDto : IMapFrom<TModel>
    {
        if (args == null)
            return;

        if (!string.IsNullOrWhiteSpace(args.Filter))
        {
            var e = DynamicExpressionParser.ParseLambda<TDto, bool>(
                new ParsingConfig{DateTimeIsParsedAsUTC = true},
                true,
                args.Filter);

            var mappedQuery = mapper.Map<Expression<Func<TModel, bool>>>(e);

            spec.And(mappedQuery);
        }

        if (!string.IsNullOrWhiteSpace(args.OrderBy))
        {
            var parts = args.OrderBy.Split();
            var propertyName = parts.First();

            var param = Expression.Parameter(typeof(TDto), "x");
            Expression conversion = Expression.Convert(Expression.Property(param, propertyName), typeof(object));
            var e = Expression.Lambda<Func<TDto, object>>(conversion, param);

            var mappedQuery = mapper.Map<Expression<Func<TModel, object>>>(e);

            if (parts[1] == "asc")
                spec.ApplyOrderBy(mappedQuery);
            else
                spec.ApplyOrderByDescending(mappedQuery);
        }
    }
}