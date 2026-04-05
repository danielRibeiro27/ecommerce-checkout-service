using EcommerceCheckoutService.Application.Services;
using EcommerceCheckoutService.Infra.Queue;
using System.Text.Json;

namespace EcommerceCheckoutService.Worker;

public class Worker : BackgroundService
{
    private readonly IQueueConsumer _queueConsumer;
    private readonly CheckoutService _checkoutService;
    private readonly ILogger<Worker> _logger;

    public Worker(IQueueConsumer queueConsumer, CheckoutService checkoutService, ILogger<Worker> logger)
    {
        _queueConsumer = queueConsumer;
        _checkoutService = checkoutService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queueConsumer.ConsumeAsync(async (message, ct) =>
        {
            PaymentStatusMessage? payload = null;
            try
            {
                payload = JsonSerializer.Deserialize<PaymentStatusMessage>(message);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize queue message: {Message}", message);
                return;
            }

            if (payload is null)
            {
                _logger.LogWarning("Queue message deserialized to null: {Message}", message);
                return;
            }

            await _checkoutService.UpdatePaymentStatusAsync(payload.OrderId, payload.Status);
        }, stoppingToken);
    }
}

record PaymentStatusMessage(Guid OrderId, string Status);
