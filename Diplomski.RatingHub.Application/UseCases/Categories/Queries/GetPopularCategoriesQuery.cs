using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Domain.Models;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Queries;

public class GetPopularCategoriesQuery : IRequest<IEnumerable<PopularCategoryDto>>
{
    
}

public class GetPopularCategoriesQueryHandler : IRequestHandler<GetPopularCategoriesQuery, IEnumerable<PopularCategoryDto>>
{
    private readonly IDatabaseRepository<Category>  _categoryRepository;
    private readonly ICompanyRepository  _companyRepository;

    public GetPopularCategoriesQueryHandler(IDatabaseRepository<Category> categoryRepository,
        ICompanyRepository companyRepository)
    {
        _categoryRepository = categoryRepository;
        _companyRepository = companyRepository;
    }

    public async Task<IEnumerable<PopularCategoryDto>> Handle(GetPopularCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories =
            await _categoryRepository.GetAndProject<PopularCategoryDto>(
                new Specification<Category>(c => c.ShowOnHomePage));

        foreach (var category in categories)
        {
            category.PopularCompanies =
                await _companyRepository.GetPopularCompaniesAndProject<PopularCompanyDto>(0, category.Id, 10);
        }

        return categories;
    }
}

public class PopularCategoryDto : IMapFrom<Category>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Icon { get; set; }
    public int NumberOfCompanies { get; set; }
    public IEnumerable<PopularCompanyDto> PopularCompanies { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Category, PopularCategoryDto>()
            .ForMember(dest => dest.NumberOfCompanies, 
                opt => opt.MapFrom(src => src.Companies.Count()));
    }
}