using MediatR;

namespace RestaurantDashboard.Application.Employees.Commands.DeleteEmployee;

public sealed record DeleteEmployeeCommand(Guid EmployeeId) : IRequest;
