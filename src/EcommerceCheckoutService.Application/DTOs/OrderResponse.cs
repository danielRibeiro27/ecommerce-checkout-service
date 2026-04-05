using EcommerceCheckoutService.Domain.Entities;

namespace EcommerceCheckoutService.Application.DTOs;

public record OrderResponse(Order Order, List<PaymentIntent> PaymentIntents);
