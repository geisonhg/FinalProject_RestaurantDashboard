using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Infrastructure.Persistence.Configurations;

public sealed class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.EmployeeId).IsRequired();
        builder.Property(s => s.Date).IsRequired();
        builder.Property(s => s.ClockIn).IsRequired();
        builder.Property(s => s.ClockOut).IsRequired(false);
        builder.Property(s => s.TipsEarned).HasPrecision(10, 2).HasDefaultValue(0m);
        builder.Property(s => s.Status).IsRequired().HasConversion<int>();

        builder.HasIndex(s => s.EmployeeId);
        builder.HasIndex(s => s.Date).IsDescending();

        builder.Ignore(s => s.Duration);

        builder.ToTable("Shifts");
    }
}
