using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Queries;

public class GetSubcategoriesQuery : IRequest<IPaginatedList<SubcategoryDto>>
{
    public int ParentCategoryId { get; set; }
    public QueryArgs? QueryArgs { get; set; }
}

public class GetSubcategoriesQueryValidator : AbstractValidator<GetSubcategoriesQuery>
{
    public GetSubcategoriesQueryValidator()
    {
        RuleFor(x => x.ParentCategoryId).GreaterThan(0).WithMessage("CategoryId mora biti veci od nule");
    }
}

public class GetSubcategoriesQueryHandler : IRequestHandler<GetSubcategoriesQuery, IPaginatedList<SubcategoryDto>>
{
    private readonly IDatabaseRepository<Category> _categoryRepository;

    public GetSubcategoriesQueryHandler(IDatabaseRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IPaginatedList<SubcategoryDto>> Handle(GetSubcategoriesQuery request, CancellationToken cancellationToken)
    {
        var specification = new Specification<Category>(c => c.ParentId == request.ParentCategoryId);
        return await _categoryRepository.GetAndProjectAsPaginatedList<SubcategoryDto>(specification, request.QueryArgs);
    }
}
public class SubcategoryDto : IMapFrom<Category>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int SortOrder { get; set; }
    public bool HasChildren { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Category, SubcategoryDto>()
            .ForMember(dest => dest.HasChildren,
                options => options.MapFrom((src) => 
                    src.Subcategories.Any()));
    }
}