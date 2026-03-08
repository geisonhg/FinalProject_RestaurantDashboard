using RestaurantDashboard.Domain.Common;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.ValueObjects;

namespace RestaurantDashboard.Domain.Entities;

public sealed class Sale : AggregateRoot
{
    public Guid OrderId { get; private set; }
    public DateOnly Date { get; private set; }
    public Money Subtotal { get; private set; } = default!;
    public Money TaxAmount { get; private set; } = default!;
    public Money TipAmount { get; private set; } = default!;
    public Money TotalAmount { get; private set; } = default!;
    public PaymentMethod PaymentMethod { get; private set; }
    public Guid ProcessedByEmployeeId { get; private set; }

    public Tip? Tip { get; private set; }

    private Sale() { }

    internal static Sale Create(
        Guid orderId,
        Money subtotal,
        decimal taxRate,
        decimal tipAmount,
        PaymentMethod paymentMethod,
        Guid processedByEmployeeId)
    {
        Guard.AgainstNegative(tipAmount, nameof(tipAmount));

        var tax = Money.From(subtotal.Amount * taxRate);
        var tip = Money.From(tipAmount);
        var total = subtotal + tax + tip;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Date = date,
            Subtotal = subtotal,
            TaxAmount = tax,
            TipAmount = tip,
            TotalAmount = total,
            PaymentMethod = paymentMethod,
            ProcessedByEmployeeId = processedByEmployeeId
        };

        if (tipAmount > 0)
            sale.Tip = Tip.Create(sale.Id, processedByEmployeeId, tipAmount, date);

        return sale;
    }
}
