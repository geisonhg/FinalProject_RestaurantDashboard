using MediatR;
using RestaurantDashboard.Application.Employees.Dtos;
using RestaurantDashboard.Domain.Enums;

namespace RestaurantDashboard.Application.Employees.Commands.CreateEmployee;

public sealed record CreateEmployeeCommand : IRequest<EmployeeDto>
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public EmployeeRole Role { get; init; }
    public DateOnly HireDate { get; init; }
}
