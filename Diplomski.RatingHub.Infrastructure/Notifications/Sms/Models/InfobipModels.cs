namespace Diplomski.RatingHub.Infrastructure.Notifications.Sms.Models;

public sealed class SendSmsRequest
{
    public List<SmsMessageDto> Messages { get; init; } = new();
}

public sealed class SmsMessageDto
{
    public List<DestinationDto> Destinations { get; init; } = new();
    public string Sender { get; init; } = "";
    public ContentDto Content { get; init; } = new();
}

public sealed class DestinationDto
{
    public string To { get; init; } = "";
}

public sealed class ContentDto
{
    public string Text { get; init; } = "";
}