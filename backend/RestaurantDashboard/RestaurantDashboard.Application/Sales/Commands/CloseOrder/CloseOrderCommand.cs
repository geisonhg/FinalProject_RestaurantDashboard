using MediatR;
using RestaurantDashboard.Domain.Enums;

namespace RestaurantDashboard.Application.Sales.Commands.CloseOrder;

public sealed record CloseOrderCommand : IRequest<Guid>
{
    public Guid OrderId { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public decimal TipAmount { get; init; }
}
