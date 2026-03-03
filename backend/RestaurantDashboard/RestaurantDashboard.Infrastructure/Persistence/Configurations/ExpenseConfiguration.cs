using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Infrastructure.Persistence.Configurations;

public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Category).IsRequired().HasConversion<int>();
        builder.Property(e => e.Date).IsRequired();
        builder.Property(e => e.Description).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.ReceiptUrl).HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.RecordedByEmployeeId).IsRequired();
        builder.Property(e => e.IsApproved).IsRequired().HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.OwnsOne(e => e.Amount, m =>
        {
            m.Property(p => p.Amount).HasColumnName("Amount").HasPrecision(10, 2);
            m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3).HasDefaultValue("EUR");
        });

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(e => e.RecordedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.Date).IsDescending();
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.IsApproved);

        builder.Ignore(e => e.DomainEvents);

        builder.ToTable("Expenses");
    }
}
