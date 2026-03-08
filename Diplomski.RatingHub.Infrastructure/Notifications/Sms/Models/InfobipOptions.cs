namespace Diplomski.RatingHub.Infrastructure.Notifications.Sms.Models;

public sealed class InfobipOptions
{
    public const string SectionName = "Notifications:Sms:Infobip";

    public string BaseUrl { get; init; } = "https://m939v9.api.infobip.com"; 
    public string ApiKey { get; init; } = "";
    public string Sender { get; init; } = "";
}