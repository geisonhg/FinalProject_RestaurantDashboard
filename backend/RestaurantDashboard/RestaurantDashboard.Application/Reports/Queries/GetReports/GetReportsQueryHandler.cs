using MediatR;
using RestaurantDashboard.Application.Reports.Dtos;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Reports.Queries.GetReports;

public sealed class GetReportsQueryHandler
    : IRequestHandler<GetReportsQuery, IReadOnlyList<ReportDto>>
{
    private readonly IReportRepository _reports;

    public GetReportsQueryHandler(IReportRepository reports) => _reports = reports;

    public async Task<IReadOnlyList<ReportDto>> Handle(
        GetReportsQuery request,
        CancellationToken cancellationToken)
    {
        var reports = await _reports.GetAllAsync(cancellationToken);

        return reports.Select(r => new ReportDto
        {
            Id = r.Id,
            Type = r.Type.ToString(),
            PeriodStart = r.PeriodStart,
            PeriodEnd = r.PeriodEnd,
            FilePath = r.FilePath,
            GeneratedAt = r.GeneratedAt,
        }).ToList().AsReadOnly();
    }
}
