using EcommerceCheckoutService.Application.Services;
using EcommerceCheckoutService.Domain.Entities;
using EcommerceCheckoutService.Infra.Queue;
using EcommerceCheckoutService.Infra.Repositories.Interface;
using EcommerceCheckoutService.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IOrderRepository, NoOpOrderRepository>();
builder.Services.AddSingleton<IPaymentIntentRepository, NoOpPaymentIntentRepository>();
builder.Services.AddSingleton<IEventPublisher, NoOpEventPublisher>();
builder.Services.AddSingleton<IQueueConsumer, NoOpQueueConsumer>();
builder.Services.AddScoped<CheckoutService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

class NoOpOrderRepository : IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id) => Task.FromResult<Order?>(null);
    public Task AddAsync(Order order) => Task.CompletedTask;

    Task<Order> IOrderRepository.AddAsync(Order order)
    {
        throw new NotImplementedException();
    }
}

class NoOpPaymentIntentRepository : IPaymentIntentRepository
{
    public Task<PaymentIntent?> GetByOrderIdAsync(Guid orderId) => Task.FromResult<PaymentIntent?>(null);
    public Task AddAsync(PaymentIntent paymentIntent) => Task.CompletedTask;

    Task<List<PaymentIntent>> IPaymentIntentRepository.GetByOrderIdAsync(Guid orderId)
    {
        throw new NotImplementedException();
    }

    Task<PaymentIntent> IPaymentIntentRepository.AddAsync(PaymentIntent paymentIntent)
    {
        throw new NotImplementedException();
    }
}

class NoOpEventPublisher : IEventPublisher
{
    public Task PublishAsync<T>(string topic, T message) => Task.CompletedTask;
}

class NoOpQueueConsumer : IQueueConsumer
{
    public Task ConsumeAsync(Func<string, CancellationToken, Task> handler, CancellationToken cancellationToken)
        => Task.Delay(Timeout.Infinite, cancellationToken);
}
