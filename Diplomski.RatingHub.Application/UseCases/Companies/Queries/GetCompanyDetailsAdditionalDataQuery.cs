using AutoMapper;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Queries;

public class GetCompanyDetailsAdditionalDataQuery : IRequest<CompanyDetailsAdditionalDataDto>
{
    public int CompanyId { get; set; }
}

public class GetCompanyDetailsAdditionalDataQueryValidator : AbstractValidator<GetCompanyDetailsAdditionalDataQuery>
{
    public GetCompanyDetailsAdditionalDataQueryValidator()
    {
        RuleFor(x => x.CompanyId).GreaterThan(0);
    }
}

public class GetCompanyDetailsAdditionalDataQueryHandler : IRequestHandler<GetCompanyDetailsAdditionalDataQuery, CompanyDetailsAdditionalDataDto>
{
    private readonly IDatabaseRepository<Company> _companyRepository;

    public GetCompanyDetailsAdditionalDataQueryHandler(IDatabaseRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<CompanyDetailsAdditionalDataDto> Handle(GetCompanyDetailsAdditionalDataQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetSingleAndProject<CompanyDetailsAdditionalDataDto>(new Specification<Company>(c => c.Id == request.CompanyId));
        if (company is null)
            throw new AppException("Kompanija ne postoji");
        
        return company;
    }
}

public class CompanyDetailsAdditionalDataDto : IMapFrom<Company>
{
    public int Id { get; set; }
    public double OverallAverageGrade { get; set; }
    public int ReviewsCount { get; set; }
    public int OneStarReviewsCount { get; set; }
    public int TwoStarReviewsCount { get; set; }
    public int ThreeStarReviewsCount { get; set; }
    public int FourStarReviewsCount { get; set; }
    public int FiveStarReviewsCount { get; set; }
    public int VerifiedReviewsCount { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<CompanyRatingAggregateDto> RatingAggregates { get; set; } = new();
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Company, CompanyDetailsAdditionalDataDto>()
            .ForMember(dest => dest.OneStarReviewsCount,
                opt => opt.MapFrom(src => src.Reviews.Count(r => r.OverallScore <= 1.5)))
            .ForMember(dest => dest.TwoStarReviewsCount,
                opt => opt.MapFrom(src => src.Reviews.Count(r => r.OverallScore > 1.5 && r.OverallScore <= 2.5)))
            .ForMember(dest => dest.ThreeStarReviewsCount,
                opt => opt.MapFrom(src => src.Reviews.Count(r => r.OverallScore > 2.5 && r.OverallScore <= 3.5)))
            .ForMember(dest => dest.FourStarReviewsCount,
                opt => opt.MapFrom(src => src.Reviews.Count(r => r.OverallScore > 3.5 && r.OverallScore <= 4.5)))
            .ForMember(dest => dest.FiveStarReviewsCount,
                opt => opt.MapFrom(src => src.Reviews.Count(r => r.OverallScore > 4.5 && r.OverallScore <= 5)))
            .ForMember(dest => dest.VerifiedReviewsCount,
                opt => opt.MapFrom(src => src.Reviews.Count(r => r.ReviewerId != null)))
            .ForMember(dest => dest.RatingAggregates,
                opt => opt.MapFrom(src => src.CompanyRatingAggregates));
    }
}

public class CompanyRatingAggregateDto : IMapFrom<CompanyRatingAggregate>
{
    public string Name { get; set; }
    public double AverageValue { get; set; }
    public int SortOrder { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CompanyRatingAggregate, CompanyRatingAggregateDto>()
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.RatingCriterion.Name))
            .ForMember(dest => dest.SortOrder,
                opt => opt.MapFrom(src => src.RatingCriterion.SortOrder));
    }
}