using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Queries;

public class GetAllTopCategoriesQuery : IRequest<IEnumerable<TopCategoryDto>>
{
}

public class GetAllTopCategoriesQueryHandler : IRequestHandler<GetAllTopCategoriesQuery, IEnumerable<TopCategoryDto>>
{
    private readonly IDatabaseRepository<Category>  _categoryRepository;

    public GetAllTopCategoriesQueryHandler(IDatabaseRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<TopCategoryDto>> Handle(GetAllTopCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetAndProject<TopCategoryDto>(
            new Specification<Category>(c => c.ParentId == null).ApplyOrderBy(c => c.SortOrder));
    }
}

public class TopCategoryDto : IMapFrom<Category>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Icon { get; set; }
    public bool HasChildren { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Category, TopCategoryDto>()
            .ForMember(dest => dest.HasChildren,
                options => options.MapFrom((src) => 
                    src.Subcategories.Any()));
    }
}