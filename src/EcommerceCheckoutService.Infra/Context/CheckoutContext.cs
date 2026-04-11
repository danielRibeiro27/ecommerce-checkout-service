using Microsoft.EntityFrameworkCore;
using EcommerceCheckoutService.Domain.Entities;

namespace EcommerceCheckoutService.Infra.Context;

public class CheckoutDbContext(DbContextOptions<CheckoutDbContext> options) : DbContext(options)
{
    public DbSet<PaymentIntent> PaymentIntents { get; set; } 
    public DbSet<Order> Orders { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PaymentIntent>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.OrderId).IsRequired();
            entity.Property(p => p.Status).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Amount).IsRequired();
            entity.Property(p => p.Currency).IsRequired().HasMaxLength(3);
            entity.Property(p => p.CreatedAt).IsRequired();
            entity.Property(p => p.IdempotencyKey).IsRequired();
            entity.HasIndex(p => p.IdempotencyKey).IsUnique(); // Unique constraint for idempotency
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Status).IsRequired().HasMaxLength(50);
            entity.Property(o => o.Amount).IsRequired();
            entity.Property(o => o.Currency).IsRequired().HasMaxLength(3);
        });
    }
}