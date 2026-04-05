using EcommerceCheckoutService.Application.DTOs;
using EcommerceCheckoutService.Domain.Entities;
using EcommerceCheckoutService.Infra.Queue;
using EcommerceCheckoutService.Infra.Repositories.Interface;

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

    public async Task<OrderResponse> CreateCheckoutAsync(decimal amount, string currency)
    {
        var order = new Order
        {
            Status = "Pending",
            Amount = amount,
            Currency = currency
        };

        var createdOrder = await _orderRepository.AddAsync(order);

        var paymentIntent = new PaymentIntent
        {
            OrderId = createdOrder.Id,
            Status = "Created",
            IdempotencyKey = Guid.NewGuid().ToString()
        };

        var createdPaymentIntent = await _paymentIntentRepository.AddAsync(paymentIntent);
        await _eventPublisher.PublishAsync("checkout.created", createdOrder);

        return new OrderResponse(createdOrder, [createdPaymentIntent]);
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
