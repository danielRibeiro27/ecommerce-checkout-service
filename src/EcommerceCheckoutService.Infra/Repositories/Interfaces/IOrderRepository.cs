using EcommerceCheckoutService.Domain.Entities;

namespace EcommerceCheckoutService.Infra.Repositories.Interface;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order> AddAsync(Order order);
}
