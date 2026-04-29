using Diplomski.RatingHub.Application.Interfaces.Notifications;
using Diplomski.RatingHub.Application.Models.Notifications;

namespace Diplomski.RatingHub.Infrastructure.Notifications.Email;

internal sealed class EmailNotificationService : IEmailNotificationService
{
    private readonly IBrevoClient _brevo;

    public EmailNotificationService(IBrevoClient brevo)
    {
        _brevo = brevo;
    }
    
    public Task SendConfirmationLinkAsync(string email, string confirmationLink)
        => SendAsync(EmailTemplates.ConfirmEmail(email, confirmationLink));
    
    public Task SendResetPasswordLinkAsync(string email, string link)
        => SendAsync(EmailTemplates.ResetPassword(email, link));
    
    public Task SendCompanyVerificationRulesAsync(string email, string identifier, string companyName)
        => SendAsync(EmailTemplates.CompanyVerificationRules(email, identifier, companyName));

    public Task SendAsync(EmailMessage message, CancellationToken ct = default)
        => _brevo.SendTransactionalEmailAsync(message, ct);
    
}