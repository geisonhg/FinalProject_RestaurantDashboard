using Microsoft.AspNetCore.SignalR;

namespace RestaurantDashboard.Web.Hubs;

/// <summary>
/// SignalR hub that external clients (or JS tabs) can connect to
/// in order to receive real-time order change notifications.
/// Method pushed: "orderChanged"
/// </summary>
public sealed class OrderHub : Hub;
