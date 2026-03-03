namespace RestaurantDashboard.Web.Services;

/// <summary>
/// Scoped Blazor state service for sharing dashboard selection state
/// across components within the same circuit (SignalR connection).
/// </summary>
public sealed class DashboardStateService
{
    public DateOnly SelectedFrom { get; private set; } =
        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-6));

    public DateOnly SelectedTo { get; private set; } =
        DateOnly.FromDateTime(DateTime.UtcNow);

    public event Action? OnChange;

    public void SetDateRange(DateOnly from, DateOnly to)
    {
        SelectedFrom = from;
        SelectedTo = to;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
