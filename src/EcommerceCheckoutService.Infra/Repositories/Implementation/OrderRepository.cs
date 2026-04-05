using EcommerceCheckoutService.Domain.Entities;
using EcommerceCheckoutService.Infra.Context;
using EcommerceCheckoutService.Infra.Repositories.Interface;

namespace EcommerceCheckoutService.Infra.Repositories.Implementation;

public class OrderRepository(CheckoutDbContext dbContext) : IOrderRepository
{
    private readonly CheckoutDbContext _dbContext = dbContext;
    public async Task<Order> AddAsync(Order order)
    {
        var result = await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public Task<Order?> GetByIdAsync(Guid id)
    {
        return _dbContext.Orders.FindAsync(id).AsTask();
    }
}