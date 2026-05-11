using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Queries;

public class GetPopularCompaniesQuery : IRequest<IEnumerable<PopularCompanyDto>>
{
    public int CityId { get; set; }
    public int CategoryId { get; set; }
    public int Take { get; set; }
}

public class GetPopularCompaniesQueryValidator : AbstractValidator<GetPopularCompaniesQuery>
{
    public GetPopularCompaniesQueryValidator()
    {
        RuleFor(x => x.CityId).NotNull();
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("CategoryId mora biti veci od nule");
        RuleFor(x => x.Take).GreaterThan(0).WithMessage("Take mora biti veci od 0");
    }
}

public class GetPopularCompaniesQueryHandler : IRequestHandler<GetPopularCompaniesQuery, IEnumerable<PopularCompanyDto>>
{
    private readonly ICompanyRepository _companyRepository;

    public GetPopularCompaniesQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<IEnumerable<PopularCompanyDto>> Handle(GetPopularCompaniesQuery request, CancellationToken cancellationToken)
    {
        return await _companyRepository.GetPopularCompaniesAndProject<PopularCompanyDto>(request.CityId, request.CategoryId, request.Take);
    }
}

public class PopularCompanyDto : IMapFrom<Company>
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
    public string? ProfileImagePath { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Company, PopularCompanyDto>()
            .ForMember(dest => dest.City, 
                opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.ProfileImagePath, 
                opt => opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsProfile).Path));
    }
}