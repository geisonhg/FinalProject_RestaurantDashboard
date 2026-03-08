using MediatR;
using RestaurantDashboard.Application.Employees.Dtos;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Employees.Queries.GetEmployeePerformance;

public sealed class GetEmployeePerformanceQueryHandler
    : IRequestHandler<GetEmployeePerformanceQuery, IReadOnlyList<EmployeePerformanceDto>>
{
    private readonly IEmployeeRepository _employees;
    private readonly ISaleRepository _sales;

    public GetEmployeePerformanceQueryHandler(
        IEmployeeRepository employees,
        ISaleRepository sales)
    {
        _employees = employees;
        _sales = sales;
    }

    public async Task<IReadOnlyList<EmployeePerformanceDto>> Handle(
        GetEmployeePerformanceQuery request,
        CancellationToken cancellationToken)
    {
        var employeesTask = _employees.GetAllActiveAsync(cancellationToken);
        var salesTask = _sales.GetByDateRangeAsync(request.From, request.To, cancellationToken);

        await Task.WhenAll(employeesTask, salesTask);

        var allEmployees = employeesTask.Result;
        var allSales = salesTask.Result;

        var salesByEmployee = allSales
            .GroupBy(s => s.ProcessedByEmployeeId)
            .ToDictionary(
                g => g.Key,
                g => (Count: g.Count(), Revenue: g.Sum(s => s.TotalAmount.Amount)));

        return allEmployees.Select(e =>
        {
            salesByEmployee.TryGetValue(e.Id, out var saleData);

            return new EmployeePerformanceDto
            {
                EmployeeId = e.Id,
                FullName = e.FullName,
                Role = e.Role.ToString(),
                TotalShifts = e.GetTotalShifts(request.From, request.To),
                TotalHoursWorked = e.GetTotalHours(request.From, request.To),
                TotalOrdersServed = saleData.Count,
                TotalSalesAmount = saleData.Revenue,
                TotalTipsEarned = e.GetTotalTips(request.From, request.To)
            };
        }).ToList();
    }
}
