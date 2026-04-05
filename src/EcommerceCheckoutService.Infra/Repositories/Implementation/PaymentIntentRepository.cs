using EcommerceCheckoutService.Domain.Entities;
using EcommerceCheckoutService.Infra.Context;
using EcommerceCheckoutService.Infra.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EcommerceCheckoutService.Infra.Repositories.Implementation;

public class PaymentIntentRepository(CheckoutDbContext dbContext) : IPaymentIntentRepository
{
    private readonly CheckoutDbContext _dbContext = dbContext;

    public async Task<PaymentIntent> AddAsync(PaymentIntent paymentIntent)
    {
        var result = await _dbContext.PaymentIntents.AddAsync(paymentIntent);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public Task<List<PaymentIntent>> GetByOrderIdAsync(Guid orderId)
    {
        return _dbContext.PaymentIntents.Where(pi => pi.OrderId == orderId).ToListAsync();
    }
}