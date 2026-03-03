using MediatR;
using RestaurantDashboard.Application.Sales.Dtos;

namespace RestaurantDashboard.Application.Sales.Commands.CreateOrder;

public sealed record CreateOrderCommand : IRequest<OrderDto>
{
    public int TableNumber { get; init; }
    public Guid EmployeeId { get; init; }
    public string? Notes { get; init; }
}
