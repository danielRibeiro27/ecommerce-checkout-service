using EcommerceCheckoutService.Domain.Entities;

namespace EcommerceCheckoutService.Infra.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task AddAsync(Order order);
}
