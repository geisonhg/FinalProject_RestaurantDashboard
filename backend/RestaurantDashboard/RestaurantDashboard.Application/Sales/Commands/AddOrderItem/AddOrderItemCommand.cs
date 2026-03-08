using MediatR;

namespace RestaurantDashboard.Application.Sales.Commands.AddOrderItem;

public sealed record AddOrderItemCommand : IRequest<Unit>
{
    public Guid OrderId { get; init; }
    public Guid MenuItemId { get; init; }
    public int Quantity { get; init; }
}
