using MediatR;
using RestaurantDashboard.Application.Sales.Dtos;

namespace RestaurantDashboard.Application.Sales.Queries.GetOrderById;

public sealed record GetOrderByIdQuery : IRequest<OrderDto>
{
    public Guid OrderId { get; init; }
}
