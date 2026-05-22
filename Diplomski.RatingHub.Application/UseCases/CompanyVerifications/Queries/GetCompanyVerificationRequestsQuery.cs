using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.CompanyVerifications.Queries;

public class GetCompanyVerificationRequestsQuery : IRequest<IPaginatedList<CompanyVerificationRequestDto>>
{
    public required string Search { get; set; }
    public CompanyVerificationRequestStatus? Status { get; set; }
    public QueryArgs QueryArgs { get; set; }
}

public class GetCompanyVerificationRequestsQueryValidator : AbstractValidator<GetCompanyVerificationRequestsQuery>
{
    public GetCompanyVerificationRequestsQueryValidator()
    {
        RuleFor(x => x.QueryArgs).NotNull();
    }
}

public class GetCompanyVerificationRequestsQueryHandler : IRequestHandler<GetCompanyVerificationRequestsQuery, IPaginatedList<CompanyVerificationRequestDto>>
{
    private readonly IDatabaseRepository<CompanyVerificationRequest> _repo;

    public GetCompanyVerificationRequestsQueryHandler(IDatabaseRepository<CompanyVerificationRequest> repo)
    {
        _repo = repo;
    }

    public async Task<IPaginatedList<CompanyVerificationRequestDto>> Handle(GetCompanyVerificationRequestsQuery request, CancellationToken cancellationToken)
    {
        var spec = new Specification<CompanyVerificationRequest>(
                x => x.ContactEmail.ToLower().Contains(request.Search) || 
                     x.Company.Name.ToLower().Contains(request.Search))
            .AddInclude(x => x.Company);

        if (request.Status.HasValue)
        {
            spec.And(x => x.Status == request.Status);
        }

        return await _repo.GetAndProjectAsPaginatedList<CompanyVerificationRequestDto>(spec, request.QueryArgs);
    }
}

public class CompanyVerificationRequestDto : IMapFrom<CompanyVerificationRequest>
{
    public int Id { get; set; }

    public CompanyVerificationRequestStatus Status { get; set; }
    public string? Description { get; set; }
    public string? ContactEmail { get; set; }
    public string? Identifier { get; set; }
    
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string CompanyVerifier { get; set; }
    public string CompanyFullAddress { get; set; }
    public string? CompanyUrl { get; set; }
    public int OwnerId { get; set; }
    public string? UserVerifier { get; set; }
    public string? UserProfileImagePath { get; set; }
    public string? UserFullName { get; set; }
    
    public DateTime Created { get; set; }
    //public CompanyDto Company { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CompanyVerificationRequest, CompanyVerificationRequestDto>()
            .ForMember(dest => dest.CompanyName, opt => 
                opt.MapFrom(src => src.Company.Name))
            .ForMember(dest => dest.CompanyVerifier, opt => 
                opt.MapFrom(src => src.Company.Verifier))
            .ForMember(dest => dest.CompanyFullAddress, opt => 
                opt.MapFrom(src => src.Company.City.Name+", "+src.Company.Location+", "+src.Company.Street+" "+src.Company.HouseNumber))
            .ForMember(dest => dest.UserVerifier, opt => 
                opt.MapFrom(src => src.Owner.PhoneNumber ?? src.Owner.Email))
            .ForMember(dest => dest.UserProfileImagePath, opt => 
                opt.MapFrom(src => src.Owner.ProfileImage.Path))
            .ForMember(dest => dest.UserFullName, opt => 
                opt.MapFrom(src => src.Owner.Name+" "+src.Owner.Surname))
            .ForMember(dest => dest.CompanyUrl, opt => 
                opt.MapFrom(src => "/companies/"+ src.CompanyId));
    }

}