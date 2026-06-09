namespace Diplomski.RatingHub.Infrastructure.Payments.Models;

public class StripeOptions
{
    public const string SectionName = "Stripe";
    public string SecretKey { get; set; } = "";
}