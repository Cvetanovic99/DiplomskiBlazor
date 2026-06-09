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

    public static EmailMessage CompanyVerificationRules(string toEmail, string identifier, string companyName) =>
        new(
            To: new EmailRecipient(toEmail),
            Subject: $"Zahtev za potvrdu kompanije: #{identifier} na platformi 'Kriterijum'",
            HtmlBody: $"""
                           <p>Hvala na zapocetom procesu verifikacije vase kompanije.</p>
                           <p>Nakon verifikacije korisnici ce imati vise poverenje u informacije o vasoj kompaniji.</p>
                           <p>Odgovorom na ovaj mail nam prosledite video zapis kojim cemo da utvrdimo da ste vi pravi vlasnik kompanije:{companyName}</p>
                           <p>Ako ste registrovana firma mozete snimiti:</p>
                           <p>  1. APR obrazac o firmi.</p>
                           <p>  2. Ako radite sa fizickim licima, fiskalnu kasu i racune koje ste izdavali</p>
                           <p>  3. Racun za struju ili komunalne usluge na ime firme.</p>
                           <p>  4. Izadjite ispred firme i snimite natpis.</p>
                           <p>Ako nemate registrovanu firmu mozete snimiti:</p>
                           <p>  1. Alat kojim obavaljate vasu delatnost.</p>
                           <p>  2. Prostorije u kojima obavaljate delatnost.</p>
                           <p>  3. Dokument kao dokaz o vasem imenu i prezimenu.</p>
                           <p>  4. Snimite naziv ulice i broj objekta gde radite</p>
                           <p>Nakon sto utvrdimo validnost podataka vasa kompanija ce biti verifikovana. Ako je utvdjivanje otezano preko video snimaka mozete biti zakazan video poziv.</p>
                       """
        );
    
    public static EmailMessage NotificationAboutCompanyCreation(string toEmail, string companyName, string claimCompanyIdentifier) =>
        new(
            To: new EmailRecipient(toEmail),
            Subject: $"Obavestenje o vasoj komaniji: {companyName} na platformi 'Kriterijum'",
            HtmlBody: $"""
                           <p>Na nasoj platformi je kreirana vasa kompanija {companyName}.</p>
                           <p>Od sada ce ljudi kojima ste pruzili usluge moci da ostave misljenje o saradnji sa vama</p>
                           <p>Ukoliko zelite mozete preuzeti vlasnistvo nad kompanijom i takodje verifikovati vlasnistvo</p>
                           <p>Sve sto je potrebno da uradite je da kreirate profil na nasoj platformi <a href="http://localhost:5141">Kriterijum</a></p>
                           <p>Nakon kreiranja profila na stranici "Moje kompanije" ubacite ovaj kod: {claimCompanyIdentifier} i klikom na dugme "Preuzmi kompaniju" bicete u mogucnosti da editujete podatke o vasoj kompaniji i odgovarati na komentare.</p>
                           <p>Ako vi niste vlasnik kompanije {companyName} zanemarite ovu poruku.</p>
                       """
        );
    
    public static EmailMessage NotificationAboutSponsoredCompanyExpiration(string toEmail, string companyName, string expirationDate) =>
        new(
            To: new EmailRecipient(toEmail),
            Subject: $"Obavestenje o isteku sponzorisane kompanije: {companyName} na platformi 'Kriterijum'",
            HtmlBody: $"""
                           <p>Postovani vasoj kompaniji: '{companyName}' istice sponzorisanje: {expirationDate}.</p>
                           <p>Ukoliko zelite mozete pre isteka ili nakon toga opet izvrsiti uplatu za sponzorisanje kompanije</p>
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
    
    public static EmailMessage SponsoredCompanyExpiration(string companyName, string expirationDate) =>//Just for testing purposes
        new(
            To: new EmailRecipient("cvetanovicgoran99@gmail.com"),
            Subject: $"Obavestenje o isteku sponzorisane kompanije: {companyName} na platformi 'Kriterijum'",
            HtmlBody: $"""
                           <p>Postovani vasoj kompaniji: '{companyName}' istice sponzorisanje: {expirationDate}.</p>
                           <p>Ukoliko zelite mozete pre isteka ili nakon toga opet izvrsiti uplatu za sponzorisanje kompanije</p>
                       """
        );
}