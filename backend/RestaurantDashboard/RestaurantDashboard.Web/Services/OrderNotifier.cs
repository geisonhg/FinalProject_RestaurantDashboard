namespace RestaurantDashboard.Web.Services;

/// <summary>
/// Singleton service that broadcasts order-change events to all active Blazor circuits.
/// Components subscribe to <see cref="OnChange"/> and call InvokeAsync(StateHasChanged).
/// </summary>
public sealed class OrderNotifier
{
    public event Action? OnChange;

    public void Notify() => OnChange?.Invoke();
}
