using MediatR;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Expenses.Commands.RecordExpense;

public sealed class RecordExpenseCommandHandler : IRequestHandler<RecordExpenseCommand, Guid>
{
    private readonly IExpenseRepository _expenses;
    private readonly IUnitOfWork _uow;

    public RecordExpenseCommandHandler(IExpenseRepository expenses, IUnitOfWork uow)
    {
        _expenses = expenses;
        _uow = uow;
    }

    public async Task<Guid> Handle(RecordExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = Expense.Record(
            request.Category,
            request.Amount,
            request.Date,
            request.Description,
            request.RecordedByEmployeeId,
            request.ReceiptUrl);

        await _expenses.AddAsync(expense, cancellationToken);
        await _uow.CommitAsync(cancellationToken);
        return expense.Id;
    }
}
