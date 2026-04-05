using EcommerceCheckoutService.Domain.Entities;

namespace EcommerceCheckoutService.Infra.Repositories;

public interface IPaymentIntentRepository
{
    Task<PaymentIntent?> GetByOrderIdAsync(Guid orderId);
    Task AddAsync(PaymentIntent paymentIntent);
}
