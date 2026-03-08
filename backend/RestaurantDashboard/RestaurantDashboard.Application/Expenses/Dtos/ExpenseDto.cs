namespace RestaurantDashboard.Application.Expenses.Dtos;

public sealed record ExpenseDto
{
    public Guid Id { get; init; }
    public string Category { get; init; } = default!;
    public decimal Amount { get; init; }
    public DateOnly Date { get; init; }
    public string Description { get; init; } = default!;
    public string? ReceiptUrl { get; init; }
    public Guid RecordedByEmployeeId { get; init; }
    public bool IsApproved { get; init; }
    public DateTime CreatedAt { get; init; }
}
