using RestaurantDashboard.Domain.Common;

namespace RestaurantDashboard.Domain.Events;

public sealed record OrderClosedEvent(
    Guid OrderId,
    Guid SaleId,
    decimal TotalAmount) : DomainEvent;
