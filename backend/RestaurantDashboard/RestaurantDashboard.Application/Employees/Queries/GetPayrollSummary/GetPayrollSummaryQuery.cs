using MediatR;
using RestaurantDashboard.Application.Employees.Dtos;

namespace RestaurantDashboard.Application.Employees.Queries.GetPayrollSummary;

public sealed record GetPayrollSummaryQuery : IRequest<IReadOnlyList<EmployeePayrollDto>>
{
    public DateOnly From { get; init; }
    public DateOnly To { get; init; }
}
