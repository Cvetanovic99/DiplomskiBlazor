namespace Diplomski.RatingHub.Application.Models.Notifications;

public sealed record SmsMessage(string ToPhoneNumber, string Text);