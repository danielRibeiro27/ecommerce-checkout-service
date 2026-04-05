namespace EcommerceCheckoutService.Infra.Queue;

public interface IEventPublisher
{
    Task PublishAsync<T>(string topic, T message);
}
