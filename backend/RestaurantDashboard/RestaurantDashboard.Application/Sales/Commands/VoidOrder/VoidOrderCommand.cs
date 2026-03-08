using MediatR;

namespace RestaurantDashboard.Application.Sales.Commands.VoidOrder;

public sealed record VoidOrderCommand : IRequest<Unit>
{
    public Guid OrderId { get; init; }
    public string Reason { get; init; } = default!;
}
