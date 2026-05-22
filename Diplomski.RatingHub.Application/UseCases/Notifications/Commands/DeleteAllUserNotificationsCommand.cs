using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Notifications.Commands;

public class DeleteAllUserNotificationsCommand : IRequest<Unit>
{
    public int UserProfileId { get; set; }
}

public class DeleteAllUserNotificationsCommandValidator : AbstractValidator<DeleteAllUserNotificationsCommand>
{
    public DeleteAllUserNotificationsCommandValidator()
    {
        RuleFor(x => x.UserProfileId).GreaterThan(0);
    }
}

public class DeleteAllUserNotificationsCommandHandler : IRequestHandler<DeleteAllUserNotificationsCommand, Unit>
{
    private readonly IDatabaseRepository<Notification> _repository;

    public DeleteAllUserNotificationsCommandHandler(IDatabaseRepository<Notification> repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteAllUserNotificationsCommand request, CancellationToken cancellationToken)
    {
        var notifications = await _repository.Get(new Specification<Notification>(n => n.RecipientId == request.UserProfileId));

        if (notifications.Any())
        {
            await _repository.DeleteRange(notifications);
        }

        return Unit.Value;
    }
}