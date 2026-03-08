using RestaurantDashboard.Application.Reports.Dtos;
using RestaurantDashboard.Application.Sales.Dtos;
using RestaurantDashboard.Application.Employees.Dtos;

namespace RestaurantDashboard.Application.Common.Interfaces;

public interface IReportService
{
    Task<string> GenerateWeeklyReportAsync(
        SalesSummaryDto summary,
        IEnumerable<EmployeePerformanceDto> staffPerformance,
        CancellationToken ct = default);

    Task<string> GenerateMonthlyReportAsync(
        SalesSummaryDto summary,
        IEnumerable<EmployeePerformanceDto> staffPerformance,
        CancellationToken ct = default);
}
