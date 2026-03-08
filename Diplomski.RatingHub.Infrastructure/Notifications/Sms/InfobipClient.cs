using System.Net.Http.Json;
using Diplomski.RatingHub.Application.Models.Notifications;
using Diplomski.RatingHub.Infrastructure.Notifications.Sms.Models;
using Microsoft.Extensions.Options;

namespace Diplomski.RatingHub.Infrastructure.Notifications.Sms;

internal interface IInfobipClient
{
    Task SendSmsAsync(SmsMessage message, CancellationToken ct);
}

internal sealed class InfobipClient : IInfobipClient
{
    private readonly HttpClient _http;
    private readonly InfobipOptions _opt;
    
    private const string SmsEndpoint = "/sms/3/messages";

    public InfobipClient(HttpClient http, IOptions<InfobipOptions> opt)
    {
        _http = http;
        _opt = opt.Value;
    }

    public async Task SendSmsAsync(SmsMessage message, CancellationToken ct)
    {
        var payload = new SendSmsRequest
        {
            Messages = new List<SmsMessageDto>
            {
                new()
                {
                    Destinations = new List<DestinationDto>
                    {
                        new() { To = NormalizeToDigits(message.ToPhoneNumber) }
                    },
                    Sender = _opt.Sender,         
                    Content = new ContentDto
                    {
                        Text = message.Text 
                    }
                }
            }
        };
        
        using var resp = await _http.PostAsJsonAsync(SmsEndpoint, payload, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException(
                $"Infobip SMS failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
        }
    }

    private static string NormalizeToDigits(string localSerbianMobile)
    {
        // Input is guaranteed to match ^06[0-9]{7,8}$
        // Example: 0641234567 -> 381641234567
        return "381" + localSerbianMobile[1..];
    }
}