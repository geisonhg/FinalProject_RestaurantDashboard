using MediatR;
using RestaurantDashboard.Domain.Enums;

namespace RestaurantDashboard.Application.Expenses.Commands.RecordExpense;

public sealed record RecordExpenseCommand : IRequest<Guid>
{
    public ExpenseCategory Category { get; init; }
    public decimal Amount { get; init; }
    public DateOnly Date { get; init; }
    public string Description { get; init; } = default!;
    public Guid RecordedByEmployeeId { get; init; }
    public string? ReceiptUrl { get; init; }
}
