using Diplomski.RatingHub.Application.Interfaces.Repositories;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Queries;

public class GetCategoriesWithBreadCrumbQuery : IRequest<IEnumerable<CategoryWithBreadCrumbDto>>
{
    public string FilterValue { get; set; } 
    public int Take { get; set; }
}

public class GetCategoriesWithBreadCrumbQueryValidator : AbstractValidator<GetCategoriesWithBreadCrumbQuery>
{
    public GetCategoriesWithBreadCrumbQueryValidator()
    {
        RuleFor(x => x.FilterValue).NotNull();
        RuleFor(x => x.Take).GreaterThan(0).LessThanOrEqualTo(20);
    }
}

public class GetCategoriesWithBreadCrumbQueryHandler : IRequestHandler<GetCategoriesWithBreadCrumbQuery, IEnumerable<CategoryWithBreadCrumbDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    
    public GetCategoriesWithBreadCrumbQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    public async Task<IEnumerable<CategoryWithBreadCrumbDto>> Handle(GetCategoriesWithBreadCrumbQuery request, CancellationToken cancellationToken)
    {
        return await  _categoryRepository.GetCategoriesWithBreadCrumbs(request.FilterValue, request.Take); 
    }
}

public class CategoryWithBreadCrumbDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FullPath { get; set; } = string.Empty;
}

public class CategoryNode
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}