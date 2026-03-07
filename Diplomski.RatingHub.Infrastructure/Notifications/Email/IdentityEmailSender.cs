using Diplomski.RatingHub.Application.Interfaces.Notifications;
using Diplomski.RatingHub.Application.Models.Notifications;
using Microsoft.AspNetCore.Identity;

namespace Diplomski.RatingHub.Infrastructure.Notifications.Email;

public sealed class IdentityEmailSender<TUser> : IEmailSender<TUser> where TUser : class
{
    private readonly IEmailNotificationService _email;

    public IdentityEmailSender(IEmailNotificationService email)
    {
        _email = email;
    }

    public Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink)
        => _email.SendAsync(EmailTemplates.ConfirmEmail(email, confirmationLink));

    public Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink)
        => _email.SendAsync(new EmailMessage(
            To: new EmailRecipient(email),
            Subject: "Reset your password",
            HtmlBody: $"""
                           <p>You can reset your password by clicking the link below:</p>
                           <p><a href="{resetLink}">Reset password</a></p>
                       """));

    public Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode)
        => _email.SendAsync(new EmailMessage(
            To: new EmailRecipient(email),
            Subject: "Your password reset code",
            HtmlBody: $"""
                           <p>Your password reset code is:</p>
                           <p><strong>{resetCode}</strong></p>
                       """));
}