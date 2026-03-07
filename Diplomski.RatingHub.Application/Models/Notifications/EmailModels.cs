namespace Diplomski.RatingHub.Application.Models.Notifications;

public sealed record EmailRecipient(string Email, string? Name = null);

public sealed record EmailMessage(
    EmailRecipient To,
    string Subject,
    string HtmlBody,
    string? TextBody = null,
    string? ReplyToEmail = null,
    string? ReplyToName = null
);