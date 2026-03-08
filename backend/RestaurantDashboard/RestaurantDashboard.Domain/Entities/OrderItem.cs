using RestaurantDashboard.Domain.Common;
using RestaurantDashboard.Domain.ValueObjects;

namespace RestaurantDashboard.Domain.Entities;

public sealed class OrderItem : Entity
{
    public Guid OrderId { get; private set; }
    public Guid MenuItemId { get; private set; }
    public string MenuItemName { get; private set; } = default!;  // Snapshot for historical accuracy
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = default!;      // Snapshot at time of order
    public Money LineTotal => UnitPrice * Quantity;

    private OrderItem() { }

    internal static OrderItem Create(Guid orderId, Guid menuItemId, string menuItemName, int quantity, Money unitPrice)
    {
        Guard.AgainstNullOrEmpty(menuItemName, nameof(menuItemName));
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));

        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            MenuItemId = menuItemId,
            MenuItemName = menuItemName,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    internal void UpdateQuantity(int quantity)
    {
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));
        Quantity = quantity;
    }
}
