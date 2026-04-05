using EcommerceCheckoutService.Application.DTOs;
using EcommerceCheckoutService.Domain.Entities;
using EcommerceCheckoutService.Infra.Logging;
using EcommerceCheckoutService.Infra.Queue;
using EcommerceCheckoutService.Infra.Repositories.Interface;

namespace EcommerceCheckoutService.Application.Services;

public class CheckoutService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentIntentRepository _paymentIntentRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IAppLogger _logger;

    public CheckoutService(
        IOrderRepository orderRepository,
        IPaymentIntentRepository paymentIntentRepository,
        IEventPublisher eventPublisher,
        IAppLogger logger)
    {
        _orderRepository = orderRepository;
        _paymentIntentRepository = paymentIntentRepository;
        _eventPublisher = eventPublisher;
        _logger = logger;
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
        _logger.Info($"Order created: {createdOrder.Id}");

        var paymentIntent = new PaymentIntent
        {
            OrderId = createdOrder.Id,
            Status = "Created",
            Amount = createdOrder.Amount.ToString(),
            Currency = createdOrder.Currency,
            CreatedAt = DateTime.UtcNow,
            IdempotencyKey = Guid.NewGuid().ToString()
        };

        var createdPaymentIntent = await _paymentIntentRepository.AddAsync(paymentIntent);
        _logger.Info($"PaymentIntent created: {createdPaymentIntent.Id} for order: {createdOrder.Id}");

        await _eventPublisher.PublishAsync("checkout.created", createdOrder);

        return new OrderResponse(createdOrder, [createdPaymentIntent]);
    }

    public async Task UpdatePaymentStatusAsync(Guid orderId, string status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
        {
            _logger.Warning($"Order not found for status update: {orderId}");
            return;
        }

        order.Status = status;
        await _orderRepository.AddAsync(order);
        _logger.Info($"Order {orderId} status updated to: {status}");
    }
}
