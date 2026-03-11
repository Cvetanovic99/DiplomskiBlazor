using Diplomski.RatingHub.Application.Models.Notifications;

namespace Diplomski.RatingHub.Application.Interfaces.Notifications;

public interface IEmailNotificationService
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
    Task SendConfirmationLinkAsync(string email, string confirmationLink);
}