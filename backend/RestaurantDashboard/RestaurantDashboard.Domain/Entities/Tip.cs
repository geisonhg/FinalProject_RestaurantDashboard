using RestaurantDashboard.Domain.Common;
using RestaurantDashboard.Domain.ValueObjects;

namespace RestaurantDashboard.Domain.Entities;

public sealed class Tip : Entity
{
    public Guid SaleId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Money Amount { get; private set; } = default!;
    public DateOnly Date { get; private set; }

    private Tip() { }

    internal static Tip Create(Guid saleId, Guid employeeId, decimal amount, DateOnly date)
    {
        Guard.AgainstNegative(amount, nameof(amount));
        return new Tip
        {
            Id = Guid.NewGuid(),
            SaleId = saleId,
            EmployeeId = employeeId,
            Amount = Money.From(amount),
            Date = date
        };
    }
}
