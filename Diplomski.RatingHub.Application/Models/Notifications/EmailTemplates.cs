namespace Diplomski.RatingHub.Application.Models.Notifications;

public static class EmailTemplates
{
    public static EmailMessage ConfirmEmail(string toEmail, string confirmationLink) =>
        new(
            To: new EmailRecipient(toEmail),
            Subject: "Confirm your email",
            HtmlBody: $"""
                           <p>Thanks for registering.</p>
                           <p>Please confirm your email by clicking the link below:</p>
                           <p><a href="{confirmationLink}">Confirm email</a></p>
                           <p>If you didn’t request this, you can ignore this message.</p>
                       """
        );

    public static EmailMessage ConfirmToken(string token) =>
        new(
            To: new EmailRecipient("cvetanovicgoran99@gmail.com"),
            Subject: "Potvrda broja telefona",
            HtmlBody: $"""
                           <p>Kod za potvdu broja je: {token} </p>
                       """
        );
}