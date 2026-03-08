using MediatR;
using Microsoft.AspNetCore.SignalR;
using RestaurantDashboard.Domain.Events;
using RestaurantDashboard.Web.Services;

namespace RestaurantDashboard.Web.Hubs.EventHandlers;

/// <summary>
/// Handles all order-state domain events and:
///  1. Notifies Blazor circuits via <see cref="OrderNotifier"/> (C# event).
///  2. Broadcasts "orderChanged" to any external SignalR clients connected to <see cref="OrderHub"/>.
/// </summary>
public sealed class OrderChangedEventHandler :
    INotificationHandler<OrderOpenedEvent>,
    INotificationHandler<OrderClosedEvent>,
    INotificationHandler<OrderVoidedEvent>
{
    private readonly OrderNotifier _notifier;
    private readonly IHubContext<OrderHub> _hub;

    public OrderChangedEventHandler(OrderNotifier notifier, IHubContext<OrderHub> hub)
    {
        _notifier = notifier;
        _hub = hub;
    }

    public Task Handle(OrderOpenedEvent notification, CancellationToken ct) => NotifyAll(ct);
    public Task Handle(OrderClosedEvent notification, CancellationToken ct) => NotifyAll(ct);
    public Task Handle(OrderVoidedEvent notification, CancellationToken ct) => NotifyAll(ct);

    private async Task NotifyAll(CancellationToken ct)
    {
        _notifier.Notify();
        await _hub.Clients.All.SendAsync("orderChanged", ct);
    }
}
