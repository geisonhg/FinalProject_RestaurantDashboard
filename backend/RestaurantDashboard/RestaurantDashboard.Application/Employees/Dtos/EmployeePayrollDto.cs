namespace RestaurantDashboard.Application.Employees.Dtos;

public sealed record EmployeePayrollDto
{
    public Guid EmployeeId { get; init; }
    public string FullName { get; init; } = default!;
    public string Role { get; init; } = default!;
    public int ShiftCount { get; init; }
    public double TotalHours { get; init; }
    public decimal TotalTips { get; init; }
}
