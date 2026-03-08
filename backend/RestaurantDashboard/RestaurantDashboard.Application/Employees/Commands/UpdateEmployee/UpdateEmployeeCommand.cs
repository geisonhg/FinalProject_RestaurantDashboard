using MediatR;
using RestaurantDashboard.Application.Employees.Dtos;
using RestaurantDashboard.Domain.Enums;

namespace RestaurantDashboard.Application.Employees.Commands.UpdateEmployee;

public sealed record UpdateEmployeeCommand : IRequest<EmployeeDto>
{
    public Guid EmployeeId { get; init; }
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public EmployeeRole Role { get; init; }
    public bool IsActive { get; init; }
}
