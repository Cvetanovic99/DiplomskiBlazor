using AutoMapper;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Queries;

public class GetCompanyDetailsQuery : IRequest<CompanyDetailsDto>
{
    public int CompanyId { get; set; }
}

public class GetCompanyDetailsQueryValidator : AbstractValidator<GetCompanyDetailsQuery>
{
    public GetCompanyDetailsQueryValidator()
    {
        RuleFor(x => x.CompanyId).GreaterThan(0).WithMessage("CompanyId mora biti veci od 0");
    }
}

public class GetCompanyDetailsQueryHandler : IRequestHandler<GetCompanyDetailsQuery, CompanyDetailsDto>
{
    private readonly IDatabaseRepository<Company> _companyRepository;

    public GetCompanyDetailsQueryHandler(IDatabaseRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<CompanyDetailsDto> Handle(GetCompanyDetailsQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetSingleAndProject<CompanyDetailsDto>(
            new Specification<Company>(c => c.Id == request.CompanyId));
        if (company is null)
            throw new AppException("Trazena kompanija ne postoji");
        
        return company;
    }
}

public class CompanyDetailsDto : IMapFrom<Company>
{
    public int Id { get; set; }
    public string Name { get; set; } =  null!;
    public int ReviewsCount { get; set; }
    public double OverallAverageGrade { get; set; }
    public string? Description { get; set; }
    public int CityId { get; set; }
    public string City { get; set; } = null!;
    public string? Location { get; set; } //Maybe village name
    public string Street { get; set; } = null!;
    public string HouseNumber { get; set; } = null!;
    public string Verifier { get; set; } = null!;  //Phonenumber or Email
    public bool IsEmailVerifier { get; set; }//Is email, if not then it's phonenumber
    public string? CompanyPib { get; set; }//If company is registered
    public string? PublicPageUrl  { get; set; }//If company has some public page website, instagram, facebook
    public bool IsVerified { get; set; }//Is verified with video-admin
    public bool IsClaimed { get; set; }
    public string? AnonymousEditIdentifier { get; set; }//If someone create company anonymously and whant to edit
    public int? OwnerId { get; set; }
    public int CategoryId { get; set; }
    public string? ProfileImagePath { get; set; }
    public List<string>? Images { get; set; } = new();
    public int CompanyDataTrueCount { get; set; } 
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Company, CompanyDetailsDto>()
            .ForMember(dest => dest.City, 
                opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.ProfileImagePath, 
                opt => opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsProfile).Path))
            .ForMember(dest => dest.Images, 
                opt => opt.MapFrom(src => 
                    src.Images.Where(i => !i.IsProfile).Select(i => i.Path).ToList()))
            .ForMember(dest => dest.IsClaimed, 
                opt => opt.MapFrom(src => src.OwnerId != null))
            .ForMember(dest => dest.CompanyDataTrueCount, 
                opt => opt.MapFrom(src => src.Reviews.Count(r => r.IsCompanyDataTrue == true)));
    }
}