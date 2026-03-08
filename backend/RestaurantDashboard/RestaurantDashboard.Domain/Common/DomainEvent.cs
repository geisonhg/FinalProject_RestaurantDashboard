using MediatR;

namespace RestaurantDashboard.Domain.Common;

public abstract record DomainEvent : INotification
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
