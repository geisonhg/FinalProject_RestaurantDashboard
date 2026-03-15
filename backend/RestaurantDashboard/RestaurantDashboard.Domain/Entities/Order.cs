using RestaurantDashboard.Domain.Common;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Events;
using RestaurantDashboard.Domain.ValueObjects;

namespace RestaurantDashboard.Domain.Entities;

public sealed class Order : AggregateRoot
{
    private const decimal TaxRate = 0.23m; // Irish VAT rate

    private readonly List<OrderItem> _items = new();

    public int TableNumber { get; private set; }
    public Guid EmployeeId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? Notes { get; private set; }
    public decimal DiscountAmount { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Money Subtotal => _items.Count == 0
        ? Money.Zero()
        : _items.Aggregate(Money.Zero(), (acc, i) => acc + i.LineTotal);

    private Order() { }

    public static Order Open(int tableNumber, Guid employeeId, string? notes = null)
    {
        Guard.AgainstNegativeOrZero(tableNumber, nameof(tableNumber));

        var order = new Order
        {
            Id = Guid.NewGuid(),
            TableNumber = tableNumber,
            EmployeeId = employeeId,
            Status = OrderStatus.Open,
            OpenedAt = DateTime.UtcNow,
            Notes = notes
        };

        order.Raise(new OrderOpenedEvent(order.Id, tableNumber, employeeId));
        return order;
    }

    public void AddItem(Guid menuItemId, string menuItemName, int quantity, Money unitPrice)
    {
        Guard.AgainstOrderClosed(Status);
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));

        var existing = _items.FirstOrDefault(i => i.MenuItemId == menuItemId);
        if (existing is not null)
        {
            existing.UpdateQuantity(existing.Quantity + quantity);
        }
        else
        {
            _items.Add(OrderItem.Create(Id, menuItemId, menuItemName, quantity, unitPrice));
        }
    }

    public void RemoveItem(Guid orderItemId)
    {
        Guard.AgainstOrderClosed(Status);
        var item = _items.FirstOrDefault(i => i.Id == orderItemId)
            ?? throw new Exceptions.DomainException($"OrderItem '{orderItemId}' not found.");
        _items.Remove(item);
    }

    public Sale Close(PaymentMethod paymentMethod, decimal tipAmount, decimal discountAmount = 0)
    {
        Guard.AgainstOrderAlreadyClosed(Status);

        if (!_items.Any())
            throw new Exceptions.DomainException("Cannot close an order with no items.");

        if (discountAmount < 0) throw new Exceptions.DomainException("Discount cannot be negative.");
        if (discountAmount > Subtotal.Amount) throw new Exceptions.DomainException("Discount cannot exceed the order subtotal.");
        DiscountAmount = discountAmount;
        var discountedSubtotal = discountAmount > 0 ? Money.From(Subtotal.Amount - discountAmount) : Subtotal;

        Status = OrderStatus.Closed;
        ClosedAt = DateTime.UtcNow;

        var sale = Sale.Create(Id, discountedSubtotal, TaxRate, tipAmount, paymentMethod, EmployeeId);
        Raise(new OrderClosedEvent(Id, sale.Id, sale.TotalAmount.Amount));
        return sale;
    }

    public void Void(string reason)
    {
        Guard.AgainstOrderAlreadyClosed(Status);
        Guard.AgainstNullOrEmpty(reason, nameof(reason));
        Status = OrderStatus.Voided;
        ClosedAt = DateTime.UtcNow;
        Raise(new OrderVoidedEvent(Id));
    }
}
