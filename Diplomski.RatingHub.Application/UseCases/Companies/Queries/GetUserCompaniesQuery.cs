using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Queries;

public class GetUserCompaniesQuery : IRequest<IPaginatedList<UserCompanyDto>>
{
    public int UserProfileId { get; set; }
    public QueryArgs QueryArgs { get; set; }
}

public class GetUSerCompaniesQueryValidator : AbstractValidator<GetUserCompaniesQuery>
{
    public GetUSerCompaniesQueryValidator()
    {
        RuleFor(x => x.UserProfileId).GreaterThan(0);
        RuleFor(x => x.QueryArgs).NotNull();
    }
}

public class GetUSerCompaniesQueryHandler : IRequestHandler<GetUserCompaniesQuery, IPaginatedList<UserCompanyDto>>
{
    private readonly IDatabaseRepository<Company> _companyRepository;

    public GetUSerCompaniesQueryHandler(IDatabaseRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<IPaginatedList<UserCompanyDto>> Handle(GetUserCompaniesQuery request, CancellationToken cancellationToken)
    {
        return await _companyRepository.GetAndProjectAsPaginatedList<UserCompanyDto>(
            new Specification<Company>(c => c.OwnerId == request.UserProfileId), request.QueryArgs);
    }
}

public class UserCompanyDto : IMapFrom<Company>
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
    public bool IsSponsored { get; set; }
    public DateTime? SponsoredUntil { get; set; }
    public int? OwnerId { get; set; }
    public int CategoryId { get; set; }
    public DateTime Created { get; set; }
    public string? ProfileImagePath { get; set; }
    public List<string>? Images { get; set; } = new();
    public int CompanyDataTrueCount { get; set; } 
    public UserCompanyVerificationRequestDto? VerificationRequest { get; set; } 
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Company, UserCompanyDto>()
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
                opt => opt.MapFrom(src => src.Reviews.Count(r => r.IsCompanyDataTrue == true)))
            .ForMember(dest => dest.VerificationRequest, 
                opt => opt.MapFrom(src => src.VerificationRequests.OrderByDescending(r => r.Created).FirstOrDefault()));
    }
}

public class UserCompanyVerificationRequestDto : IMapFrom<CompanyVerificationRequest>
{
    public int Id { get; set; }
    public CompanyVerificationRequestStatus Status { get; set; }
    public string ContactEmail { get; set; }
    public string? Description { get; set; }
    public string Identifier { get; set; }
}