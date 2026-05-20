using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.UserProfiles.Queries;

public class CheckForNewNotificationsQuery : IRequest<bool>
{
    public int UserId { get; set; }
}

public class CheckForNewNotificationsQueryValidator : AbstractValidator<CheckForNewNotificationsQuery>
{
    public CheckForNewNotificationsQueryValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}

public class CheckForNewNotificationsQueryHandler : IRequestHandler<CheckForNewNotificationsQuery, bool>
{
    private readonly IDatabaseRepository<Notification> _notificationRepository;

    public CheckForNewNotificationsQueryHandler(IDatabaseRepository<Notification> notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<bool> Handle(CheckForNewNotificationsQuery request, CancellationToken cancellationToken)
    {
        int notificationsCount = await _notificationRepository.GetCount(
            new Specification<Notification>(n => n.RecipientId == request.UserId && n.IsRead == false));
        
        return notificationsCount > 0;
    }
}