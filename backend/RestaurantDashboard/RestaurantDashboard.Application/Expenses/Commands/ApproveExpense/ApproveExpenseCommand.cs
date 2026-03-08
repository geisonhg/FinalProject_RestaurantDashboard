using MediatR;

namespace RestaurantDashboard.Application.Expenses.Commands.ApproveExpense;

public sealed record ApproveExpenseCommand : IRequest<Unit>
{
    public Guid ExpenseId { get; init; }
}
