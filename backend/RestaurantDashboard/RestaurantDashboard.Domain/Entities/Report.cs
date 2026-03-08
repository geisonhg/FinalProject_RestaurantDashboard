using RestaurantDashboard.Domain.Common;
using RestaurantDashboard.Domain.Enums;

namespace RestaurantDashboard.Domain.Entities;

public sealed class Report : AggregateRoot
{
    public ReportType Type { get; private set; }
    public DateOnly PeriodStart { get; private set; }
    public DateOnly PeriodEnd { get; private set; }
    public string FilePath { get; private set; } = default!;
    public Guid GeneratedByEmployeeId { get; private set; }
    public DateTime GeneratedAt { get; private set; }

    private Report() { }

    public static Report Create(
        ReportType type,
        DateOnly periodStart,
        DateOnly periodEnd,
        string filePath,
        Guid generatedByEmployeeId)
    {
        Guard.AgainstNullOrEmpty(filePath, nameof(filePath));

        if (periodEnd < periodStart)
            throw new Exceptions.DomainException("Period end cannot be before period start.");

        return new Report
        {
            Id = Guid.NewGuid(),
            Type = type,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            FilePath = filePath,
            GeneratedByEmployeeId = generatedByEmployeeId,
            GeneratedAt = DateTime.UtcNow
        };
    }
}
