namespace Diplomski.RatingHub.Application.Models.Notifications;

public static class EmailTemplates
{
    public static EmailMessage ConfirmEmail(string toEmail, string confirmationLink) =>
        new(
            To: new EmailRecipient(toEmail),
            Subject: "Confirm your email",
            HtmlBody: $"""
                           <p>Hvala za registraciju na nasoj platformi RatingHub.</p>
                           <p>Molimo vas da potvrdite email klikom na link ispod:</p>
                           <p><a href="{confirmationLink}">Potvrdi email</a></p>
                           <p>Ako vi niste izvrsili registraciju na nasoj platformi, ovu poruku mozete ignorisati.</p>
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