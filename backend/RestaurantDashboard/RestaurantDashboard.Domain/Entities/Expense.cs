using RestaurantDashboard.Domain.Common;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.ValueObjects;

namespace RestaurantDashboard.Domain.Entities;

public sealed class Expense : AggregateRoot
{
    public ExpenseCategory Category { get; private set; }
    public Money Amount { get; private set; } = default!;
    public DateOnly Date { get; private set; }
    public string Description { get; private set; } = default!;
    public string? ReceiptUrl { get; private set; }
    public Guid RecordedByEmployeeId { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Expense() { }

    public static Expense Record(
        ExpenseCategory category,
        decimal amount,
        DateOnly date,
        string description,
        Guid recordedByEmployeeId,
        string? receiptUrl = null)
    {
        Guard.AgainstNullOrEmpty(description, nameof(description));

        return new Expense
        {
            Id = Guid.NewGuid(),
            Category = category,
            Amount = Money.From(amount),
            Date = date,
            Description = description,
            RecordedByEmployeeId = recordedByEmployeeId,
            ReceiptUrl = receiptUrl,
            IsApproved = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Approve() => IsApproved = true;
    public void Revoke()  => IsApproved = false;

    public void AttachReceipt(string url)
    {
        Guard.AgainstNullOrEmpty(url, nameof(url));
        ReceiptUrl = url;
    }
}
