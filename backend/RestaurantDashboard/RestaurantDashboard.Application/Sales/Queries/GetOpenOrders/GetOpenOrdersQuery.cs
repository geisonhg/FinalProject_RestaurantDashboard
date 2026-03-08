using MediatR;
using RestaurantDashboard.Application.Sales.Dtos;

namespace RestaurantDashboard.Application.Sales.Queries.GetOpenOrders;

public sealed record GetOpenOrdersQuery : IRequest<IReadOnlyList<OrderDto>>;
