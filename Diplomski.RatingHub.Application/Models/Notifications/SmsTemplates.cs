namespace Diplomski.RatingHub.Application.Models.Notifications;

public static class SmsTemplates
{
    public static SmsMessage ConfirmPhoneNumber(string toPhoneNumber, string confirmationToken) =>
        new(
            ToPhoneNumber: toPhoneNumber,
            Text: $"Ovo je vas kod za potvrdu broja telefona: {confirmationToken}"
        );
}