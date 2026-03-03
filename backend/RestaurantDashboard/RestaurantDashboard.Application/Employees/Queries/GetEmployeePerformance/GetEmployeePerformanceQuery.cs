using MediatR;
using RestaurantDashboard.Application.Employees.Dtos;

namespace RestaurantDashboard.Application.Employees.Queries.GetEmployeePerformance;

public sealed record GetEmployeePerformanceQuery : IRequest<IReadOnlyList<EmployeePerformanceDto>>
{
    public DateOnly From { get; init; }
    public DateOnly To { get; init; }
}
