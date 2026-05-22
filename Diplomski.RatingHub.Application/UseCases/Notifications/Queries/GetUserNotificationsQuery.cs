using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.UserProfiles.Queries;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Notifications.Queries;

public class GetUserNotificationsQuery : IRequest<IPaginatedList<NotificationDto>>
{
    public int UserProfileId { get; set; }
    public QueryArgs QueryArgs { get; set; }
}

public class GetUserNotificationsQueryValidator : AbstractValidator<GetUserNotificationsQuery>
{
    public GetUserNotificationsQueryValidator()
    {
        RuleFor(x => x.UserProfileId).GreaterThan(0);
    }
}

public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, IPaginatedList<NotificationDto>>
{
    private readonly IDatabaseRepository<Notification> _notificationsRepository;

    public GetUserNotificationsQueryHandler(IDatabaseRepository<Notification> notificationsRepository)
    {
        _notificationsRepository = notificationsRepository;
    }

    public async Task<IPaginatedList<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        var unreadNotifications = await _notificationsRepository.Get(
            new Specification<Notification>(n => n.RecipientId == request.UserProfileId && n.IsRead == false));
        if (unreadNotifications.Any())
        {
            unreadNotifications.ForEach(n => n.IsRead = true);
            await _notificationsRepository.UpdateRange(unreadNotifications);
        }

        return await _notificationsRepository.GetAndProjectAsPaginatedList<NotificationDto>(
            new Specification<Notification>(n => n.RecipientId == request.UserProfileId), request.QueryArgs);
    }
}

public class NotificationDto : IMapFrom<Notification>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; set; }
}