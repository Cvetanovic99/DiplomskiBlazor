using Diplomski.RatingHub.Application.Interfaces.Payments;
using Diplomski.RatingHub.Infrastructure.Payments.Models;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Diplomski.RatingHub.Infrastructure.Payments;

public class PaymentService : IPaymentService
{
    private readonly StripeOptions _options;
    
    public PaymentService(IOptions<StripeOptions> options)
    {
        _options = options.Value;
        StripeConfiguration.ApiKey = _options.SecretKey;
    }
    
    public async Task<string> CreateCheckoutSession(int companyId)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "payment",
            SuccessUrl = $"http://localhost:5141/user/companies/payment-success?companyId={companyId}",
            CancelUrl = "http://localhost:5141/user/companies/payment-cancel",

            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = 1000, // 10€
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Sponzorisanje kompanije"
                        }
                    }
                }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return session.Url;
    }
}