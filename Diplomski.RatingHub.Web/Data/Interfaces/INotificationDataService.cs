using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Notifications.Queries;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface INotificationDataService
{
    Task<bool> CheckForNewNotifications(int userId);
    Task<IPaginatedList<NotificationDto>> GetUserNotifications(int userProfileId, QueryArgs queryArgs);
    Task DeleteAllUserNotifications(int userProfileId);
}