using MediatR;
using RestaurantDashboard.Application.Employees.Dtos;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Employees.Queries.GetPayrollSummary;

public sealed class GetPayrollSummaryQueryHandler
    : IRequestHandler<GetPayrollSummaryQuery, IReadOnlyList<EmployeePayrollDto>>
{
    private readonly IEmployeeRepository _employees;

    public GetPayrollSummaryQueryHandler(IEmployeeRepository employees) =>
        _employees = employees;

    public async Task<IReadOnlyList<EmployeePayrollDto>> Handle(
        GetPayrollSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var employees = await _employees.GetAllActiveWithShiftsAsync(cancellationToken);

        return employees
            .Select(e => new EmployeePayrollDto
            {
                EmployeeId = e.Id,
                FullName = e.FullName,
                Role = e.Role.ToString(),
                ShiftCount = e.GetTotalShifts(request.From, request.To),
                TotalHours = e.GetTotalHours(request.From, request.To),
                TotalTips = e.GetTotalTips(request.From, request.To)
            })
            .Where(e => e.ShiftCount > 0)
            .OrderByDescending(e => e.TotalHours)
            .ToList();
    }
}
