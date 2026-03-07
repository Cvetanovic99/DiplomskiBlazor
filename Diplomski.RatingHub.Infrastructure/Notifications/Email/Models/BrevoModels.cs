namespace Diplomski.RatingHub.Infrastructure.Notifications.Email.Models;

public sealed class BrevoSendEmailRequest
{
    public BrevoSender Sender { get; init; } = new();
    public List<BrevoRecipient> To { get; init; } = new();
    public string Subject { get; init; } = "";
    public string HtmlContent { get; init; } = "";
    public string? TextContent { get; init; }
    public BrevoReplyTo? ReplyTo { get; init; }
}

public sealed class BrevoSender
{
    public string Email { get; init; } = "";
    public string? Name { get; init; }
}

public sealed class BrevoRecipient
{
    public string Email { get; init; } = "";
    public string? Name { get; init; }
}

public sealed class BrevoReplyTo
{
    public string Email { get; init; } = "";
    public string? Name { get; init; }
}