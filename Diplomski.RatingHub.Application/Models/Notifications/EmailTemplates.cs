namespace Diplomski.RatingHub.Application.Models.Notifications;

public static class EmailTemplates
{
    public static EmailMessage ConfirmEmail(string toEmail, string confirmationLink) =>
        new(
            To: new EmailRecipient(toEmail),
            Subject: "Potvrda email adrese",
            HtmlBody: $"""
                           <p>Hvala za registraciju na nasoj platformi Kriterijum.</p>
                           <p>Molimo vas da potvrdite email klikom na link ispod:</p>
                           <p><a href="{confirmationLink}">Potvrdi email</a></p>
                           <p>Ako vi niste izvrsili registraciju na nasoj platformi, ovu poruku mozete ignorisati.</p>
                       """
        );
    
    public static EmailMessage ResetPassword(string toEmail, string link) =>
        new(
            To: new EmailRecipient(toEmail),
            Subject: "Resetovanje lozinke",
            HtmlBody: $"""
                           <p>Klikom na link ispod mozete nastaviti proces resetovanja lozinke:</p>
                           <p><a href="{link}">Resetuj lozinku</a></p>
                           <p>Ako vi niste zapoceli resetovanje lozinke na nasoj platformi, ovu poruku mozete ignorisati.</p>
                       """
        );

    public static EmailMessage ConfirmToken(string token) =>//Just for testing purposes
        new(
            To: new EmailRecipient("cvetanovicgoran99@gmail.com"),
            Subject: "Potvrda broja telefona",
            HtmlBody: $"""
                           <p>Kod za potvdu broja je: {token} </p>
                       """
        );
    
    public static EmailMessage ResetPasswordToken(string token) =>//Just for testing purposes
        new(
            To: new EmailRecipient("cvetanovicgoran99@gmail.com"),
            Subject: "Resetovanje lozinke",
            HtmlBody: $"""
                           <p>Kod za resetovanje lozinke je: {token} </p>
                       """
        );
}