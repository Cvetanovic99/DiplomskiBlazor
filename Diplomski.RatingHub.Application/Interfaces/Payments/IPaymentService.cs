namespace Diplomski.RatingHub.Application.Interfaces.Payments;

public interface IPaymentService
{
    Task<string> CreateCheckoutSession(int companyId);
}