using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Infrastructure.Persistence.Configurations;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.OrderId).IsRequired();
        builder.Property(s => s.Date).IsRequired();
        builder.Property(s => s.PaymentMethod).IsRequired().HasConversion<int>();
        builder.Property(s => s.ProcessedByEmployeeId).IsRequired();

        builder.OwnsOne(s => s.Subtotal, m =>
        {
            m.Property(p => p.Amount).HasColumnName("Subtotal").HasPrecision(10, 2);
            m.Property(p => p.Currency).HasColumnName("SubtotalCurrency").HasMaxLength(3).HasDefaultValue("EUR");
        });
        builder.OwnsOne(s => s.TaxAmount, m =>
        {
            m.Property(p => p.Amount).HasColumnName("TaxAmount").HasPrecision(10, 2);
            m.Property(p => p.Currency).HasColumnName("TaxCurrency").HasMaxLength(3).HasDefaultValue("EUR");
        });
        builder.OwnsOne(s => s.TipAmount, m =>
        {
            m.Property(p => p.Amount).HasColumnName("TipAmount").HasPrecision(10, 2).HasDefaultValue(0m);
            m.Property(p => p.Currency).HasColumnName("TipCurrency").HasMaxLength(3).HasDefaultValue("EUR");
        });
        builder.OwnsOne(s => s.TotalAmount, m =>
        {
            m.Property(p => p.Amount).HasColumnName("TotalAmount").HasPrecision(10, 2);
            m.Property(p => p.Currency).HasColumnName("TotalCurrency").HasMaxLength(3).HasDefaultValue("EUR");
        });

        builder.HasOne(s => s.Tip)
            .WithOne()
            .HasForeignKey<Tip>(t => t.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Order>()
            .WithOne()
            .HasForeignKey<Sale>(s => s.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(s => s.ProcessedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.Date).IsDescending();
        builder.HasIndex(s => s.ProcessedByEmployeeId);
        builder.HasIndex(s => s.OrderId).IsUnique();

        builder.Ignore(s => s.DomainEvents);

        builder.ToTable("Sales");
    }
}
