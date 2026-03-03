using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.OrderId).IsRequired();
        builder.Property(oi => oi.MenuItemId).IsRequired();
        builder.Property(oi => oi.MenuItemName).IsRequired().HasMaxLength(200);
        builder.Property(oi => oi.Quantity).IsRequired();

        builder.OwnsOne(oi => oi.UnitPrice, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("UnitPrice")
                .HasPrecision(10, 2)
                .IsRequired();
            price.Property(p => p.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .HasDefaultValue("EUR");
        });

        builder.HasIndex(oi => oi.OrderId);

        builder.Ignore(oi => oi.LineTotal);

        builder.ToTable("OrderItems");
    }
}
