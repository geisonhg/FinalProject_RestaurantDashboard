using MediatR;

namespace RestaurantDashboard.Application.Expenses.Commands.DeleteExpense;

public sealed record DeleteExpenseCommand(Guid ExpenseId) : IRequest;
