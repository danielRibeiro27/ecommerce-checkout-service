namespace EcommerceCheckoutService.Infra.Queue;

public interface IQueueConsumer
{
    Task ConsumeAsync(Func<string, CancellationToken, Task> handler, CancellationToken cancellationToken);
}
