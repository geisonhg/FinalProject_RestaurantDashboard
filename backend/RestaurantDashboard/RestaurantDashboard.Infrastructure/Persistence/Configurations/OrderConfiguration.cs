using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TableNumber).IsRequired();
        builder.Property(o => o.EmployeeId).IsRequired();
        builder.Property(o => o.Status).IsRequired().HasConversion<int>();
        builder.Property(o => o.OpenedAt).IsRequired();
        builder.Property(o => o.ClosedAt).IsRequired(false);
        builder.Property(o => o.Notes).HasMaxLength(500).IsRequired(false);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Domain.Entities.Employee>()
            .WithMany()
            .HasForeignKey(o => o.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.EmployeeId);
        builder.HasIndex(o => o.OpenedAt).IsDescending();

        builder.Ignore(o => o.Subtotal);
        builder.Ignore(o => o.DomainEvents);

        builder.ToTable("Orders");
    }
}
