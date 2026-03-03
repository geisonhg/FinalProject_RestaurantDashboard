using RestaurantDashboard.Domain.Common;
using RestaurantDashboard.Domain.ValueObjects;

namespace RestaurantDashboard.Domain.Entities;

public sealed class MenuItem : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public string Category { get; private set; } = default!;
    public Money BasePrice { get; private set; } = default!;
    public bool IsAvailable { get; private set; }

    private MenuItem() { }

    public static MenuItem Create(string name, string category, decimal basePrice)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstNullOrEmpty(category, nameof(category));

        return new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = name,
            Category = category,
            BasePrice = Money.From(basePrice),
            IsAvailable = true
        };
    }

    public void UpdatePrice(decimal newPrice) =>
        BasePrice = Money.From(newPrice);

    public void SetAvailability(bool available) =>
        IsAvailable = available;

    public void Update(string name, string category)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Guard.AgainstNullOrEmpty(category, nameof(category));
        Name = name;
        Category = category;
    }
}
