namespace RestaurantDashboard.Application.Employees.Dtos;

public sealed record EmployeePerformanceDto
{
    public Guid EmployeeId { get; init; }
    public string FullName { get; init; } = default!;
    public string Role { get; init; } = default!;
    public int TotalShifts { get; init; }
    public double TotalHoursWorked { get; init; }
    public int TotalOrdersServed { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal TotalTipsEarned { get; init; }
    public decimal AverageTipPerShift => TotalShifts > 0
        ? Math.Round(TotalTipsEarned / TotalShifts, 2) : 0m;
}
