using MediatR;
using RestaurantDashboard.Application.Sales.Dtos;

namespace RestaurantDashboard.Application.Sales.Queries.GetClosedOrders;

public sealed record GetClosedOrdersQuery : IRequest<IReadOnlyList<OrderDto>>
{
    public DateOnly From { get; init; }
    public DateOnly To { get; init; }
}
