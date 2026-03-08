using MediatR;
using RestaurantDashboard.Application.Expenses.Dtos;

namespace RestaurantDashboard.Application.Expenses.Queries.GetExpenses;

public sealed record GetExpensesQuery : IRequest<IReadOnlyList<ExpenseDto>>
{
    public DateOnly From { get; init; }
    public DateOnly To { get; init; }
}
