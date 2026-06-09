using Diplomski.RatingHub.Application.Models.Notifications;

namespace Diplomski.RatingHub.Application.Interfaces.Notifications;

public interface ISmsNotificationService
{
    Task SendConfirmationToken(string toPhoneNUmber, string token, CancellationToken ct = default);
    Task SendConfirmationTokenWithEmail(string token);
    Task SendResetPasswordTokenWithEmail(string token);
    Task NotifyOwnerAboutCompanyCreationWithEmail(string companyName, string claimCompanyIdentifier);
    Task NotifyOwnerAboutSponsoredCompanyExpiration(string companyName, string expirationDate);
}