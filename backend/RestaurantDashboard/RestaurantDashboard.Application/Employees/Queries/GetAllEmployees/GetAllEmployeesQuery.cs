using MediatR;
using RestaurantDashboard.Application.Employees.Dtos;

namespace RestaurantDashboard.Application.Employees.Queries.GetAllEmployees;

public sealed record GetAllEmployeesQuery : IRequest<IReadOnlyList<EmployeeDto>>;
