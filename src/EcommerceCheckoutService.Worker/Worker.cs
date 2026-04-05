using EcommerceCheckoutService.Application.Services;
using EcommerceCheckoutService.Infra.Logging;
using EcommerceCheckoutService.Infra.Queue;
using System.Text.Json;

namespace EcommerceCheckoutService.Worker;

public class Worker : BackgroundService
{
    private readonly IQueueConsumer _queueConsumer;
    private readonly CheckoutService _checkoutService;
    private readonly IAppLogger _logger;

    public Worker(IQueueConsumer queueConsumer, CheckoutService checkoutService, IAppLogger logger)
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
                _logger.Error($"Failed to deserialize queue message: {message}", ex);
                return;
            }

            if (payload is null)
            {
                _logger.Warning($"Queue message deserialized to null: {message}");
                return;
            }

            await _checkoutService.UpdatePaymentStatusAsync(payload.OrderId, payload.Status);
        }, stoppingToken);
    }
}

record PaymentStatusMessage(Guid OrderId, string Status);
