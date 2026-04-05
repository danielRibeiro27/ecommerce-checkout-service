using EcommerceCheckoutService.Application.Services;
using EcommerceCheckoutService.Infra.Queue;
using System.Text.Json;

namespace EcommerceCheckoutService.Worker;

public class Worker : BackgroundService
{
    private readonly IQueueConsumer _queueConsumer;
    private readonly CheckoutService _checkoutService;

    public Worker(IQueueConsumer queueConsumer, CheckoutService checkoutService)
    {
        _queueConsumer = queueConsumer;
        _checkoutService = checkoutService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queueConsumer.ConsumeAsync(async (message, ct) =>
        {
            var payload = JsonSerializer.Deserialize<PaymentStatusMessage>(message);
            if (payload is not null)
                await _checkoutService.UpdatePaymentStatusAsync(payload.OrderId, payload.Status);
        }, stoppingToken);
    }
}

record PaymentStatusMessage(Guid OrderId, string Status);
