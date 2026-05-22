using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Notifications.Commands;
using Diplomski.RatingHub.Application.UseCases.Notifications.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;

namespace Diplomski.RatingHub.Web.Data.Services;

public class NotificationDataService(IServiceScopeFactory serviceScopeFactory) : DataServiceBase(serviceScopeFactory), INotificationDataService
{
    public async Task<bool> CheckForNewNotifications(int userId)
    {
        return await Send(new CheckForNewNotificationsQuery { UserId = userId });
    }

    public async Task<IPaginatedList<NotificationDto>> GetUserNotifications(int userProfileId, QueryArgs queryArgs)
    {
        return await Send(new GetUserNotificationsQuery { UserProfileId = userProfileId, QueryArgs = queryArgs });
    }

    public async Task DeleteAllUserNotifications(int userProfileId)
    {
        await Send(new DeleteAllUserNotificationsCommand { UserProfileId = userProfileId });
    }
}