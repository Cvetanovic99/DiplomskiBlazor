using System.Net.Http.Json;
using Diplomski.RatingHub.Application.Models.Notifications;
using Diplomski.RatingHub.Infrastructure.Notifications.Email.Models;
using Microsoft.Extensions.Options;

namespace Diplomski.RatingHub.Infrastructure.Notifications.Email;

internal interface IBrevoClient
{
    Task SendTransactionalEmailAsync(EmailMessage message, CancellationToken ct);
}

internal sealed class BrevoClient : IBrevoClient
{
    private readonly HttpClient _http;
    private readonly BrevoOptions _opt;
    
    private const string EmailEndpoint = "/v3/smtp/email";

    public BrevoClient(HttpClient http, IOptions<BrevoOptions> opt)
    {
        _http = http;
        _opt = opt.Value;
    }

    public async Task SendTransactionalEmailAsync(EmailMessage message, CancellationToken ct)
    {
        var payload = new BrevoSendEmailRequest
        {
            Sender = new BrevoSender { Email = _opt.FromEmail, Name = _opt.FromName },
            To = new List<BrevoRecipient> { new() { Email = message.To.Email, Name = message.To.Name } },
            Subject = message.Subject,
            HtmlContent = message.HtmlBody,
            TextContent = message.TextBody,
            ReplyTo = BuildReplyTo(message)
            
        };

        using var resp = await _http.PostAsJsonAsync(EmailEndpoint, payload, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(
                $"Brevo send failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
        }
    }

    private BrevoReplyTo? BuildReplyTo(EmailMessage msg)
    {
        var email = msg.ReplyToEmail ?? _opt.ReplyToEmail;
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return new BrevoReplyTo
        {
            Email = email,
            Name = msg.ReplyToName ?? _opt.ReplyToName
        };
    }
}