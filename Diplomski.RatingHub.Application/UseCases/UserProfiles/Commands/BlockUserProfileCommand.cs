using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.UserProfiles.Commands;

public class BlockUserProfileCommand : IRequest<Unit>
{
    public int UserId { get; set; }
}

public class BlockUserProfileCommandValidator : AbstractValidator<BlockUserProfileCommand>
{
    public BlockUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId je obavezan");
    }
}

public class BlockUserProfileCommandHandler : IRequestHandler<BlockUserProfileCommand, Unit>
{
    private readonly IDatabaseRepository<UserProfile>  _repository;

    public BlockUserProfileCommandHandler(IDatabaseRepository<UserProfile> repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(BlockUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _repository.GetById(request.UserId);
        if (userProfile is null)
            throw new AppException("Korisnik sa trazenim Id ne postoji");

        userProfile.Blocked = true;
        await _repository.Update(userProfile);
        
        return Unit.Value;
    }
}