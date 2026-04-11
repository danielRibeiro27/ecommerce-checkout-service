using EcommerceCheckoutService.Application.DTOs;
using EcommerceCheckoutService.Application.Services;
using EcommerceCheckoutService.Infra.Context;
using EcommerceCheckoutService.Infra.Logging;
using EcommerceCheckoutService.Infra.Queue;
using EcommerceCheckoutService.Infra.Queue.Implementation;
using EcommerceCheckoutService.Infra.Repositories.Implementation;
using EcommerceCheckoutService.Infra.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IAppLogger>(_ =>
    new FileAppLogger(builder.Configuration["Logging:FilePath"] ?? "logs/api.log"));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentIntentRepository, PaymentIntentRepository>();
builder.Services.AddSingleton<IEventPublisher, SQSEventPublisher>();
builder.Services.AddDbContext<CheckoutDbContext>(options => options.UseSqlite("Data Source=memory:checkout.db"));
builder.Services.AddScoped<CheckoutService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CheckoutDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapGet("/health", () => Results.Ok("Healthy"));

app.MapPost("/checkout", async (CheckoutRequest request, CheckoutService checkoutService) =>
{
    var orderResponse = await checkoutService.CreateCheckoutAsync(request.Amount, request.Currency);
    return Results.Created($"/ordersAndPayments/{orderResponse.Order.Id}", orderResponse);
});

app.MapGet("/ordersAndPayments/{orderId:guid}", async (Guid orderId, IOrderRepository orderRepository, IPaymentIntentRepository paymentIntentRepository) =>
{
    var order = await orderRepository.GetByIdAsync(orderId);
    if (order is null)
        return Results.NotFound();

    var paymentIntents = await paymentIntentRepository.GetByOrderIdAsync(orderId);
    return Results.Ok(new OrderResponse(order, paymentIntents));
});

app.Run();
