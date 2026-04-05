namespace EcommerceCheckoutService.Domain.Entities;

public class PaymentIntent
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
}
