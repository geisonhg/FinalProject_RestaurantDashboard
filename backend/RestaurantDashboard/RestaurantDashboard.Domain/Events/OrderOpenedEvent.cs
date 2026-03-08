using RestaurantDashboard.Domain.Common;

namespace RestaurantDashboard.Domain.Events;

public sealed record OrderOpenedEvent(
    Guid OrderId,
    int TableNumber,
    Guid EmployeeId) : DomainEvent;
