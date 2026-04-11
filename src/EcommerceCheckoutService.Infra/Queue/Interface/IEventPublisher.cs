namespace EcommerceCheckoutService.Infra.Queue;

public interface IEventPublisher
{
    Task PublishAsync<T>(string queue, string topic, T message);
}
