using AutoMapper;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.UserProfiles.Queries;

public class GetCurrentUserProfileQuery : IRequest<CurrentUserProfileDto>
{
    public string IndetityId { get; set; }
}

public class GetCurrentUserProfileQueryValidator : AbstractValidator<GetCurrentUserProfileQuery>
{
    public GetCurrentUserProfileQueryValidator()
    {
        RuleFor(x => x.IndetityId).NotEmpty().WithMessage("IndetityId is required.");
    }
}

public class GetCurrentUserProfileQueryHandler : IRequestHandler<GetCurrentUserProfileQuery, CurrentUserProfileDto>
{
    private readonly IDatabaseRepository<UserProfile> _repository;

    public GetCurrentUserProfileQueryHandler(IDatabaseRepository<UserProfile> repository)
    {
        _repository = repository;
    }

    public async Task<CurrentUserProfileDto> Handle(GetCurrentUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userProfile = await _repository.GetSingleAndProject<CurrentUserProfileDto>(
            new Specification<UserProfile>(u => u.IdentityUserId == request.IndetityId));

        if (userProfile is null)
            throw new AppException("Trenutno ne postoji profil sa ovim identifikatorom");

        return userProfile;
    }
}

public class CurrentUserProfileDto : IMapFrom<UserProfile>
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public bool Blocked { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<UserProfile, CurrentUserProfileDto>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.Name + " " + src.Surname));
    }

}