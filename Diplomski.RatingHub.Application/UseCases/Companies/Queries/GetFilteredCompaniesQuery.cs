using System.Linq.Expressions;
using AutoMapper;
using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;
using Diplomski.RatingHub.Application.Specifications;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Queries;

public class GetFilteredCompaniesQuery : IRequest<IPaginatedList<FilteredCompanyDto>>
{
    public int CategoryId { get; set; }
    public int CityId { get; set; }
    public string FilterValue { get; set; }
    public QueryArgs QueryArgs { get; set; } //For orderBy and pagination
    public double OverallRatingGrade { get; set; }
    public CompanyClaimStatusFilterOptions ClaimStatus { get; set; }
    public CompanyVerificationStatusFilterOptions VerificationStatus { get; set; }
    public string OrderBy { get; set; }
    
}

public class GetFilteredCompaniesQueryValidator : AbstractValidator<GetFilteredCompaniesQuery>
{
    public GetFilteredCompaniesQueryValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId mora biti veci od 0");

        RuleFor(x => x.CityId)
            .GreaterThan(0)
            .WithMessage("CityId mora biti veci od 0");

        RuleFor(x => x.FilterValue)
            .MaximumLength(100)
            .WithMessage("FilterValue ne sme biti duzi od 100 karaktera");

        RuleFor(x => x.OverallRatingGrade)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(5)
            .WithMessage("OverallRatingGrade mora biti izmedju 0 i 5");

        RuleFor(x => x.ClaimStatus)
            .IsInEnum()
            .WithMessage("ClaimStatus mora biti validna vrednost");

        RuleFor(x => x.VerificationStatus)
            .IsInEnum()
            .WithMessage("VerificationStatus mora biti validna vrednost");
        
        RuleFor(x => x.OrderBy).NotEmpty()
            .WithMessage("Sortiranje mora biti postavljeno");
    }
}

public class GetFilteredCompaniesQueryHandler : IRequestHandler<GetFilteredCompaniesQuery, IPaginatedList<FilteredCompanyDto>>
{
    private readonly IDatabaseRepository<Company> _companyRepository;
    private readonly IDatabaseRepository<Category> _categoryRepository;

    public GetFilteredCompaniesQueryHandler(IDatabaseRepository<Company> companyRepository,
        IDatabaseRepository<Category> categoryRepository)
    {
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IPaginatedList<FilteredCompanyDto>> Handle(GetFilteredCompaniesQuery request, CancellationToken cancellationToken)
    {
        var categoryIds = await GetAllCategoryIds(request.CategoryId);

        var spec = new Specification<Company>(c =>
            categoryIds.Contains(c.CategoryId) &&
            c.CityId == request.CityId);
        
        if (!string.IsNullOrWhiteSpace(request.FilterValue))
        {
            spec.And(c => c.Name.Contains(request.FilterValue) ||
                          (c.CompanyPib != null && c.CompanyPib.Contains(request.FilterValue)));
        }

        // Filter by OverallRatingGrade
        if (request.OverallRatingGrade > 0)
        {
            spec.And(x => x.OverallAverageGrade >= request.OverallRatingGrade);
        }

        // Filter by ClaimStatus
        if (request.ClaimStatus == CompanyClaimStatusFilterOptions.Preuzete)
        {
            spec.And(c => c.OwnerId != null);
        }
        else if (request.ClaimStatus == CompanyClaimStatusFilterOptions.Nepreuzete)
        {
            spec.And(x => x.OwnerId == null);
        }

        // Filter by VerificationStatus
        if (request.VerificationStatus == CompanyVerificationStatusFilterOptions.Verifikovane)
        {
            spec.And(x => x.IsVerified == true);
        }
        else if (request.VerificationStatus == CompanyVerificationStatusFilterOptions.Neverifikovane)
        {
            spec.And(x => x.IsVerified == false);
        }

        spec.ApplyOrderByDescending(c => c.IsSponsored);
        
        if (request.OrderBy.Split(' ')[1] == "desc")
        {
            spec.ApplyThenOrderByDescending(GetSecondOrder(request.OrderBy));
        }
        else
        {
            spec.ApplyThenOrderBy(GetSecondOrder(request.OrderBy));
        }


        return await _companyRepository.GetAndProjectAsPaginatedList<FilteredCompanyDto>(spec, request.QueryArgs);
    }
    
    private async Task<List<int>> GetAllCategoryIds(int categoryId)
    {
        var allCategories = await _categoryRepository.GetAll();

        var result = new List<int>();

        void Traverse(int id)
        {
            result.Add(id);

            var children = allCategories
                .Where(c => c.ParentId == id)
                .Select(c => c.Id);

            foreach (var child in children)
                Traverse(child);
        }

        Traverse(categoryId);

        return result;
    }

    private Expression<Func<Company, object>> GetSecondOrder(string orderBy)
    {
            string value = orderBy.Split(' ')[0];
            switch (value)
            {
                case $"{nameof(FilteredCompanyDto.OverallAverageGrade)}":
                    return c => c.OverallAverageGrade;
                case $"{nameof(FilteredCompanyDto.Created)}":
                    return c => c.Created;
            }

            return c => c.Id;
    }
}

public class FilteredCompanyDto : IMapFrom<Company>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ReviewsCount { get; set; }
    public double OverallAverageGrade { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string City { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? CompanyPib { get; set; }
    public string? PublicPageUrl  { get; set; }
    public bool IsVerified { get; set; }
    public bool IsClaimed { get; set; }
    public bool IsSponsored { get; set; }
    public string? ProfileImagePath { get; set; }
    public List<string>? Images { get; set; } = new();
    public DateTime Created { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Company, FilteredCompanyDto>()
            .ForMember(dest => dest.City, 
                opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.IsClaimed, 
                opt => opt.MapFrom(src => src.OwnerId != null))
            .ForMember(dest => dest.ProfileImagePath, 
                opt => opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsProfile).Path))
            .ForMember(dest => dest.Images, 
                opt => opt.MapFrom(src => 
                    src.Images.Where(i => !i.IsProfile).Select(i => i.Path).Take(4).ToList()));
    }
}

