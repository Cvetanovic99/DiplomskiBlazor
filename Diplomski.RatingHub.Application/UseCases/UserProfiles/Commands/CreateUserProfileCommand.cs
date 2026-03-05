using AutoMapper;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.UserProfiles.Commands;

public class CreateUserProfileCommand : IRequest<UserProfileDto>
{
    public required string IdentityUserId { get; set; }
    public required string Name { get; set; } 
    public required string Surname { get; set; }
}

public class CreateUserProfileCommandValidator : AbstractValidator<CreateUserProfileCommand>
{
    public CreateUserProfileCommandValidator()
    {
        RuleFor(x => x.IdentityUserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Surname).NotEmpty();
    }
}

public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, UserProfileDto>
{
    private readonly IDatabaseRepository<UserProfile> _userProfileRepository;
    private readonly IMapper _mapper;

    public CreateUserProfileCommandHandler(
        IDatabaseRepository<UserProfile> userProfileRepository,
        IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var createdUserProfile = await _userProfileRepository
            .GetSingleBySpec(new Specification<UserProfile>(u => u.IdentityUserId == request.IdentityUserId));
        if (createdUserProfile is not null)
            throw new AppException("Korisnik je vec kreiran");

        var userProfile = new UserProfile
        {
            IdentityUserId = request.IdentityUserId,
            Name = request.Name,
            Surname = request.Surname
        };
        
        await _userProfileRepository.Insert(userProfile);
        
        return _mapper.Map<UserProfileDto>(userProfile);
    }
}

public class UserProfileDto : IMapFrom<UserProfile>
{
    public int Id { get; set; }
    public required string IdentityUserId { get; set; }
    public required string Name { get; set; } 
    public required string Surname { get; set; }
}