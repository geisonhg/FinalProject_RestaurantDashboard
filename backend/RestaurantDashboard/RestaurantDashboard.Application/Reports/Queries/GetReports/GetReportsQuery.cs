using MediatR;
using RestaurantDashboard.Application.Reports.Dtos;

namespace RestaurantDashboard.Application.Reports.Queries.GetReports;

public sealed record GetReportsQuery : IRequest<IReadOnlyList<ReportDto>>;
