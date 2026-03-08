using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Infrastructure.Persistence.Configurations;

public sealed class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Category).IsRequired().HasMaxLength(100);
        builder.Property(m => m.IsAvailable).IsRequired().HasDefaultValue(true);

        builder.OwnsOne(m => m.BasePrice, price =>
        {
            price.Property(p => p.Amount).HasColumnName("BasePrice").HasPrecision(10, 2);
            price.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3).HasDefaultValue("EUR");
        });

        builder.HasIndex(m => m.IsAvailable);
        builder.HasIndex(m => m.Category);

        builder.Ignore(m => m.DomainEvents);

        builder.ToTable("MenuItems");
    }
}
