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

public class GetCategoriesQuery : IRequest<IPaginatedList<CategoryDto>>
{
    public string FilterValue { get; set; }

    public QueryArgs QueryArgs { get; set; }
}

public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesQueryValidator()
    {
        RuleFor(x => x.QueryArgs).NotEmpty();
        RuleFor(x => x.QueryArgs.Take).LessThanOrEqualTo(20);
    }
}

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IPaginatedList<CategoryDto>>
{
    private readonly IDatabaseRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(
        IDatabaseRepository<Category> categoryRepository, 
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IPaginatedList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categorySpecification = new Specification<Category>(
            c => c.Name.Contains(request.FilterValue) ||
                 c.Keywords.Any(k => k.Keyword.Contains(request.FilterValue)));

        
        return await _categoryRepository.GetAndProjectAsPaginatedList<CategoryDto>(categorySpecification, request.QueryArgs);
    }
}

public class CategoryDto : IMapFrom<Category>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public int SortOrder { get; set; }
    public int CompaniesCount { get; set; }
    public string? Icon { get; set; }
    public bool ShowOnHomePage { get; set; }
    public string? ParentName { get; set; }
    public List<CategoryKeywordDto> Keywords { get; set; } = new();
    public List<RatingCriterionDto> RatingCriteria { get; set; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ParentName,
                options => options.MapFrom((src) =>
                    src.Parent.Name))
            .ForMember(dest => dest.CompaniesCount,
                options => options.MapFrom((src) =>
                    src.Companies.Count));
    }
}

public class CategoryKeywordDto : IMapFrom<CategoryKeyword>
{
    public int Id { get; set; }
    public string Keyword { get; set; } = null!;
    
    public int CategoryId { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CategoryKeyword, CategoryKeywordDto>()
            .ReverseMap();
    }
}

public class RatingCriterionDto : IMapFrom<RatingCriterion>
{
    public int? Id { get; set; }
    public string Name { get; set; } = null!;
    public int SortOrder { get; set; } 
    public bool IsActive { get; set; }
    public bool ContainsReview { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<RatingCriterion, RatingCriterionDto>()
            .ForMember(dest => dest.ContainsReview,
                options => options.MapFrom((src) =>
                    src.ReviewGrades.Any()));
    }
}