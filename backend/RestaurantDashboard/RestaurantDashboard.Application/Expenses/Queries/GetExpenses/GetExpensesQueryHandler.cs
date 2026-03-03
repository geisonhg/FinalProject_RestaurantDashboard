using MediatR;
using RestaurantDashboard.Application.Expenses.Dtos;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Expenses.Queries.GetExpenses;

public sealed class GetExpensesQueryHandler
    : IRequestHandler<GetExpensesQuery, IReadOnlyList<ExpenseDto>>
{
    private readonly IExpenseRepository _expenses;

    public GetExpensesQueryHandler(IExpenseRepository expenses) =>
        _expenses = expenses;

    public async Task<IReadOnlyList<ExpenseDto>> Handle(
        GetExpensesQuery request,
        CancellationToken cancellationToken)
    {
        var expenses = await _expenses.GetByDateRangeAsync(request.From, request.To, cancellationToken);

        return expenses.Select(e => new ExpenseDto
        {
            Id = e.Id,
            Category = e.Category.ToString(),
            Amount = e.Amount.Amount,
            Date = e.Date,
            Description = e.Description,
            ReceiptUrl = e.ReceiptUrl,
            RecordedByEmployeeId = e.RecordedByEmployeeId,
            IsApproved = e.IsApproved,
            CreatedAt = e.CreatedAt
        }).ToList();
    }
}
