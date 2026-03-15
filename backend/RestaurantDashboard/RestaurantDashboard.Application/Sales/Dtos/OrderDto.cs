namespace RestaurantDashboard.Application.Sales.Dtos;

public sealed record OrderDto
{
    public Guid Id { get; init; }
    public int TableNumber { get; init; }
    public string EmployeeName { get; init; } = default!;
    public string Status { get; init; } = default!;
    public DateTime OpenedAt { get; init; }
    public DateTime? ClosedAt { get; init; }
    public decimal Subtotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TipAmount { get; init; }
    public string? PaymentMethod { get; init; }
    public string? Notes { get; init; }
    public IReadOnlyList<OrderItemDto> Items { get; init; } = [];
}

public sealed record OrderItemDto
{
    public Guid Id { get; init; }
    public Guid MenuItemId { get; init; }
    public string MenuItemName { get; init; } = default!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
