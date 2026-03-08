using RestaurantDashboard.Domain.Common;

namespace RestaurantDashboard.Domain.Events;

public sealed record OrderVoidedEvent(Guid OrderId) : DomainEvent;
