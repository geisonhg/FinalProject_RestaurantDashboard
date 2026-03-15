using RestaurantDashboard.Domain.Common;
using RestaurantDashboard.Domain.ValueObjects;

namespace RestaurantDashboard.Domain.Entities;

public sealed class MenuItem : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public string Category { get; private set; } = default!;
    public string? Description { get; private set; }
    public Money BasePrice { get; private set; } = default!;
    public bool IsAvailable { get; private set; }
    public int? StockQuantity { get; private set; }

    private MenuItem() { }

    public static MenuItem Create(string name, string category, decimal basePrice, string? description = null, int? stockQuantity = null)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstNullOrEmpty(category, nameof(category));

        return new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = name,
            Category = category,
            Description = description,
            BasePrice = Money.From(basePrice),
            IsAvailable = true,
            StockQuantity = stockQuantity
        };
    }

    public void SetStock(int? quantity)
    {
        if (quantity.HasValue && quantity.Value < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");
        StockQuantity = quantity;
    }

    public void UpdatePrice(decimal newPrice) =>
        BasePrice = Money.From(newPrice);

    public void SetAvailability(bool available) =>
        IsAvailable = available;

    public void Update(string name, string category, string? description = null)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstNullOrEmpty(category, nameof(category));
        Name = name;
        Category = category;
        Description = description;
    }
}
