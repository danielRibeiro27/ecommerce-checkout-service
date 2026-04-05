using EcommerceCheckoutService.Application.DTOs;
using EcommerceCheckoutService.Application.Services;
using EcommerceCheckoutService.Domain.Entities;
using EcommerceCheckoutService.Infra.Queue;
using EcommerceCheckoutService.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IOrderRepository, NoOpOrderRepository>();
builder.Services.AddSingleton<IPaymentIntentRepository, NoOpPaymentIntentRepository>();
builder.Services.AddSingleton<IEventPublisher, NoOpEventPublisher>();
builder.Services.AddScoped<CheckoutService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/checkout", async (CheckoutRequest request, CheckoutService checkoutService) =>
{
    var order = await checkoutService.CreateCheckoutAsync(request.Amount, request.Currency);
    return Results.Ok(order);
});

app.MapGet("/orders/{id:guid}", async (Guid id, IOrderRepository orderRepository, IPaymentIntentRepository paymentIntentRepository) =>
{
    var order = await orderRepository.GetByIdAsync(id);
    if (order is null)
        return Results.NotFound();

    var paymentIntent = await paymentIntentRepository.GetByOrderIdAsync(id);
    return Results.Ok(new OrderResponse(order, paymentIntent));
});

app.Run();

class NoOpOrderRepository : IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id) => Task.FromResult<Order?>(null);
    public Task AddAsync(Order order) => Task.CompletedTask;
}

class NoOpPaymentIntentRepository : IPaymentIntentRepository
{
    public Task<PaymentIntent?> GetByOrderIdAsync(Guid orderId) => Task.FromResult<PaymentIntent?>(null);
    public Task AddAsync(PaymentIntent paymentIntent) => Task.CompletedTask;
}

class NoOpEventPublisher : IEventPublisher
{
    public Task PublishAsync<T>(string topic, T message) => Task.CompletedTask;
}
