using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Queries;

public class GetCategoriesAndCompaniesQuery : IRequest<IList<CategoryOrCompanyDto>>
{
    public string FilterValue { get; set; }
    public int CityId { get; set; }

    public QueryArgs QueryArgs { get; set; }
}

public class GetCategoriesAndCompaniesQueryValidator : AbstractValidator<GetCategoriesAndCompaniesQuery>
{
    public GetCategoriesAndCompaniesQueryValidator()
    {
        RuleFor(x => x.FilterValue).NotEmpty();
        RuleFor(x => x.QueryArgs).NotNull();
        RuleFor(x => x.CityId).GreaterThan(0);
    }
}

public class GetCategoriesAndCompaniesQueryHandler : IRequestHandler<GetCategoriesAndCompaniesQuery, IList<CategoryOrCompanyDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDatabaseRepository<Company> _companyRepository;

    public GetCategoriesAndCompaniesQueryHandler(IDatabaseRepository<Company> companyRepository,
        ICategoryRepository categoryRepository)
    {
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IList<CategoryOrCompanyDto>> Handle(GetCategoriesAndCompaniesQuery request, CancellationToken cancellationToken)
    {
        var categories= await  _categoryRepository.GetCategoriesWithBreadCrumbs(request.FilterValue, request.QueryArgs.Take!.Value);
        
        var spec = new Specification<Company>(c => c.CityId == request.CityId &&
                                                   (c.Name.Contains(request.FilterValue) ||
                                                    (c.CompanyPib != null && c.CompanyPib.Contains(request.FilterValue))))
            .ApplyOrderByDescending(x => x.OwnerId != null);
        

        var companies = await _companyRepository.GetAndProjectAsPaginatedList<CompanySearchSectionDto>(spec, new QueryArgs {Skip=0, Take = 15});
        
        var result = new List<CategoryOrCompanyDto>();
        
        result.AddRange(categories.Select(c => new CategoryOrCompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            FullPath = c.FullPath,
            IsCategory = true
        }));
        
        result.AddRange(companies.Items.Select(c => new CategoryOrCompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            ProfileImagePath = c.ProfileImagePath,
            HasOwner = c.HasOwner,
            CompanyPib = c.CompanyPib,
            IsCategory = false
        }));
        
        return result;
    }
}

public class CategoryOrCompanyDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsCategory { get; set; }
    
    //CategoryData
    public string FullPath { get; set; } = string.Empty;
    
    //Company Data
    public string? ProfileImagePath { get; set; }
    public bool HasOwner  { get; set; }
    public string? CompanyPib  { get; set; }
}

public class CompanySearchSectionDto : IMapFrom<Company>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? ProfileImagePath { get; set; }
    public bool HasOwner  { get; set; }
    public string? CompanyPib  { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Company, CompanySearchSectionDto>()
            .ForMember(dest => dest.ProfileImagePath,
                options => options.MapFrom((src) => 
                    src.Images.FirstOrDefault(i => i.IsProfile).Path))
            .ForMember(dest => dest.HasOwner,
                options => options.MapFrom(src => src.OwnerId != null));
    }
}