using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.UserProfiles.Commands;

public class DeleteUserProfileCommand : IRequest<Unit>
{
    public int UserProfileId { get; set; }
}

public class DeleteUserProfileCommandValidator : AbstractValidator<DeleteUserProfileCommand>
{
    public DeleteUserProfileCommandValidator()
    {
        RuleFor(x => x.UserProfileId).GreaterThan(0);
    }
}

public class DeleteUserProfileCommandHandler : IRequestHandler<DeleteUserProfileCommand, Unit>
{
    private readonly IDatabaseRepository<UserProfile> _userProfilesRepository;
    
    
    public async Task<Unit> Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}