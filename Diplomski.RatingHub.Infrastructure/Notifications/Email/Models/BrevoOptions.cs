namespace Diplomski.RatingHub.Infrastructure.Notifications.Email.Models;

public sealed class BrevoOptions
{
    public const string SectionName = "Notifications:Email:Brevo";

    public string ApiKey { get; init; } = "";
    public string BaseUrl { get; init; } = "https://api.brevo.com";
    public string FromEmail { get; init; } = "";
    public string FromName { get; init; } = "";

    // Optional defaults
    public string? ReplyToEmail { get; init; }
    public string? ReplyToName { get; init; }
}