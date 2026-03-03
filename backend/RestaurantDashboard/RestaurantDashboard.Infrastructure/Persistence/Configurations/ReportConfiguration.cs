using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Infrastructure.Persistence.Configurations;

public sealed class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Type).IsRequired().HasConversion<int>();
        builder.Property(r => r.PeriodStart).IsRequired();
        builder.Property(r => r.PeriodEnd).IsRequired();
        builder.Property(r => r.FilePath).IsRequired().HasMaxLength(500);
        builder.Property(r => r.GeneratedByEmployeeId).IsRequired();
        builder.Property(r => r.GeneratedAt).IsRequired();

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(r => r.GeneratedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.GeneratedAt).IsDescending();

        builder.Ignore(r => r.DomainEvents);

        builder.ToTable("Reports");
    }
}
