using EcommerceCheckoutService.Domain.Entities;

namespace EcommerceCheckoutService.Infra.Repositories.Interface;

public interface IPaymentIntentRepository
{
    Task<List<PaymentIntent>> GetByOrderIdAsync(Guid orderId);
    Task<PaymentIntent> AddAsync(PaymentIntent paymentIntent);
}
