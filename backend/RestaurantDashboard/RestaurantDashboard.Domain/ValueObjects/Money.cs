using RestaurantDashboard.Domain.Exceptions;

namespace RestaurantDashboard.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "EUR";

    private Money() { }

    public static Money From(decimal amount, string currency = "EUR")
    {
        if (amount < 0)
            throw new DomainException($"Money amount cannot be negative. Value: {amount}");
        return new Money { Amount = Math.Round(amount, 2), Currency = currency };
    }

    public static Money Zero(string currency = "EUR") => From(0, currency);

    public static Money operator +(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return From(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return From(a.Amount - b.Amount, a.Currency);
    }

    public static Money operator *(Money a, int multiplier) =>
        From(a.Amount * multiplier, a.Currency);

    public override string ToString() => $"{Currency} {Amount:N2}";

    private static void EnsureSameCurrency(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException($"Cannot operate on different currencies: '{a.Currency}' and '{b.Currency}'.");
    }
}
