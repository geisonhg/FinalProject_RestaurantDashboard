using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Infrastructure.Persistence.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.HireDate)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.UserId)
            .IsRequired(false);

        builder.HasMany(e => e.Shifts)
            .WithOne()
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.UserId).IsUnique().HasFilter("\"UserId\" IS NOT NULL");
        builder.HasIndex(e => e.IsActive);

        builder.Ignore(e => e.FullName);
        builder.Ignore(e => e.DomainEvents);

        builder.ToTable("Employees");
    }
}
