using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Infrastructure.Persistence.Configurations;

public sealed class TipConfiguration : IEntityTypeConfiguration<Tip>
{
    public void Configure(EntityTypeBuilder<Tip> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.SaleId).IsRequired();
        builder.Property(t => t.EmployeeId).IsRequired();
        builder.Property(t => t.Date).IsRequired();

        builder.OwnsOne(t => t.Amount, m =>
        {
            m.Property(p => p.Amount).HasColumnName("Amount").HasPrecision(10, 2);
            m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3).HasDefaultValue("EUR");
        });

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(t => t.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.EmployeeId);
        builder.HasIndex(t => t.Date).IsDescending();

        builder.ToTable("Tips");
    }
}
