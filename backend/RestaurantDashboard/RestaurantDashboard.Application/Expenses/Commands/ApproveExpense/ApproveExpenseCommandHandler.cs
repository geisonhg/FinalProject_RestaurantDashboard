using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Expenses.Commands.ApproveExpense;

public sealed class ApproveExpenseCommandHandler : IRequestHandler<ApproveExpenseCommand, Unit>
{
    private readonly IExpenseRepository _expenses;
    private readonly IUnitOfWork _uow;

    public ApproveExpenseCommandHandler(IExpenseRepository expenses, IUnitOfWork uow)
    {
        _expenses = expenses;
        _uow = uow;
    }

    public async Task<Unit> Handle(ApproveExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await _expenses.GetByIdAsync(request.ExpenseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Expense), request.ExpenseId);

        expense.Approve();
        _expenses.Update(expense);
        await _uow.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}
