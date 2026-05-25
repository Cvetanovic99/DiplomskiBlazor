using AutoMapper;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.UserProfiles.Queries;

public class GetUserProfileQuery : IRequest<UserProfileDto>
{
    public string IdentityUserId { get; set; }
}

public class GetUSerProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUSerProfileQueryValidator()
    {
        RuleFor(x => x.IdentityUserId).NotEmpty();
    }
}

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IDatabaseRepository<UserProfile> _repository;

    public GetUserProfileQueryHandler(IDatabaseRepository<UserProfile> repository)
    {
        _repository = repository;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userProfile =
            await _repository.GetSingleAndProject<UserProfileDto>(
                new Specification<UserProfile>(u => u.IdentityUserId == request.IdentityUserId));
        if (userProfile == null)
            throw new AppException("Korisnik ne postoji");
        
        return userProfile;
    }
}

public class UserProfileDto : IMapFrom<UserProfile>
{
    public int Id { get; set; }
    public string IdentityUserId { get; set; }
    public string Name { get; set; } 
    public string Surname { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? ProfileImagePath { get; set; }
    public bool DoesOwnCompanies { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<UserProfile, UserProfileDto>()
            .ForMember(dest => dest.ProfileImagePath,
                options => options.MapFrom((src) => src.ProfileImage.Path))
            .ForMember(dest => dest.DoesOwnCompanies,
                options => options.MapFrom((src) => src.OwningCompanies.Any()));
    }
}