using Diplomski.RatingHub.Application.Interfaces.Notifications;
using Diplomski.RatingHub.Application.Models.Notifications;

namespace Diplomski.RatingHub.Infrastructure.Notifications.Sms;

internal sealed class SmsNotificationService : ISmsNotificationService
{
    private readonly IInfobipClient _client;
    private readonly IEmailNotificationService _emailNotificationService;

    public SmsNotificationService(IInfobipClient client,  IEmailNotificationService emailNotificationService)
    {
        _client = client;
        _emailNotificationService = emailNotificationService;
    }

    public Task SendConfirmationTokenWithEmail(string token)//This is only because sms sender doesn't work
        => _emailNotificationService.SendAsync(EmailTemplates.ConfirmToken(token));

    public Task SendResetPasswordTokenWithEmail(string token)//This is only because sms sender doesn't work
    => _emailNotificationService.SendAsync(EmailTemplates.ResetPasswordToken(token));
    
    public Task NotifyOwnerAboutSponsoredCompanyExpiration(string companyName, string expirationDate)//This is only because sms sender doesn't work
        => _emailNotificationService.SendAsync(EmailTemplates.SponsoredCompanyExpiration(companyName, expirationDate));

    public Task NotifyOwnerAboutCompanyCreationWithEmail(string companyName, string claimCompanyIdentifier)
        => _emailNotificationService.SendAsync(
            EmailTemplates.NotificationAboutCompanyCreation("cvetanovicgoran99@gmail.com", companyName, claimCompanyIdentifier));

    public Task SendConfirmationToken(string toPhoneNUmber, string token, CancellationToken ct = default)
        => _client.SendSmsAsync(SmsTemplates.ConfirmPhoneNumber(toPhoneNUmber, token), ct);
}