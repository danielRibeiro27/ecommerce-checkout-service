using EcommerceCheckoutService.Domain.Entities;
using EcommerceCheckoutService.Infra.Queue;
using EcommerceCheckoutService.Infra.Repositories;

namespace EcommerceCheckoutService.Application.Services;

public class CheckoutService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentIntentRepository _paymentIntentRepository;
    private readonly IEventPublisher _eventPublisher;

    public CheckoutService(
        IOrderRepository orderRepository,
        IPaymentIntentRepository paymentIntentRepository,
        IEventPublisher eventPublisher)
    {
        _orderRepository = orderRepository;
        _paymentIntentRepository = paymentIntentRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Order> CreateCheckoutAsync(decimal amount, string currency)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = "Pending",
            Amount = amount,
            Currency = currency
        };

        await _orderRepository.AddAsync(order);

        var paymentIntent = new PaymentIntent
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            Status = "Created",
            IdempotencyKey = Guid.NewGuid().ToString()
        };

        await _paymentIntentRepository.AddAsync(paymentIntent);
        await _eventPublisher.PublishAsync("checkout.created", order);

        return order;
    }

    public async Task UpdatePaymentStatusAsync(Guid orderId, string status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
            return;

        order.Status = status;
        await _orderRepository.AddAsync(order);
    }
}
