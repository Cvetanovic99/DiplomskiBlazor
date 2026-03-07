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

    public Task SendAsync(EmailMessage message, CancellationToken ct = default)
        => _brevo.SendTransactionalEmailAsync(message, ct);
}