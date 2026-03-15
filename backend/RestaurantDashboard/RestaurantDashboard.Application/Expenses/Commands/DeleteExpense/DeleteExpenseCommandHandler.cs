using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Expenses.Commands.DeleteExpense;

public sealed class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand>
{
    private readonly IExpenseRepository _expenses;
    private readonly IUnitOfWork _uow;

    public DeleteExpenseCommandHandler(IExpenseRepository expenses, IUnitOfWork uow)
    {
        _expenses = expenses;
        _uow = uow;
    }

    public async Task Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await _expenses.GetByIdAsync(request.ExpenseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Expense), request.ExpenseId);

        _expenses.Remove(expense);
        await _uow.CommitAsync(cancellationToken);
    }
}
