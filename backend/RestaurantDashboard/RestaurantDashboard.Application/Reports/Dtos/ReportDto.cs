namespace RestaurantDashboard.Application.Reports.Dtos;

public sealed record ReportDto
{
    public Guid Id { get; init; }
    public string Type { get; init; } = default!;
    public DateOnly PeriodStart { get; init; }
    public DateOnly PeriodEnd { get; init; }
    public string FilePath { get; init; } = default!;
    public DateTime GeneratedAt { get; init; }
}
