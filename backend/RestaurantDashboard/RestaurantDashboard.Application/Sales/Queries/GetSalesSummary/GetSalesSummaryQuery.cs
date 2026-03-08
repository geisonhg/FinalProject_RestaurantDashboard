using MediatR;
using RestaurantDashboard.Application.Sales.Dtos;

namespace RestaurantDashboard.Application.Sales.Queries.GetSalesSummary;

public sealed record GetSalesSummaryQuery : IRequest<SalesSummaryDto>
{
    public DateOnly From { get; init; }
    public DateOnly To { get; init; }
}
