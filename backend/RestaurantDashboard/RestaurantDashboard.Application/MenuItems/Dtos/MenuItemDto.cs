namespace RestaurantDashboard.Application.MenuItems.Dtos;

public sealed record MenuItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Category { get; init; } = default!;
    public string? Description { get; init; }
    public decimal BasePrice { get; init; }
    public bool IsAvailable { get; init; }
    public int? StockQuantity { get; init; }
}
