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
    
    public string? ParentName { get; set; }
    public string? Keywords { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ParentName,
                options => options.MapFrom((src) =>
                    src.Parent.Name))
            .ForMember(dest => dest.Keywords,
                options => options.MapFrom((src) =>
                    string.Join(",", src.Keywords.Select(k => k.Keyword))))
            .ForMember(dest => dest.CompaniesCount,
                options => options.MapFrom((src) =>
                    src.Companies.Count));
    }
}