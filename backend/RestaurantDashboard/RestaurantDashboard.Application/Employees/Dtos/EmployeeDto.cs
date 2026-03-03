namespace RestaurantDashboard.Application.Employees.Dtos;

public sealed record EmployeeDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string FullName { get; init; } = default!;
    public string Role { get; init; } = default!;
    public DateOnly HireDate { get; init; }
    public bool IsActive { get; init; }
}
