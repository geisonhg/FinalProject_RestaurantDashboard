using FluentAssertions;
using MediatR;
using Moq;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Application.Expenses.Commands.ApproveExpense;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Tests.Expenses.Commands;

public sealed class ApproveExpenseCommandHandlerTests
{
    private readonly Mock<IExpenseRepository> _expensesMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private ApproveExpenseCommandHandler CreateSut() =>
        new(_expensesMock.Object, _uowMock.Object);

    [Fact]
    public async Task Handle_ExistingExpense_ApprovesAndReturnsUnit()
    {
        var expense = Expense.Record(ExpenseCategory.Food, 100m, DateOnly.FromDateTime(DateTime.Today), "Produce", Guid.NewGuid());
        _expensesMock.Setup(r => r.GetByIdAsync(expense.Id, default)).ReturnsAsync(expense);

        var result = await CreateSut().Handle(new ApproveExpenseCommand { ExpenseId = expense.Id }, default);

        result.Should().Be(Unit.Value);
        expense.IsApproved.Should().BeTrue();
        _uowMock.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_ExpenseNotFound_ThrowsNotFoundException()
    {
        _expensesMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Expense?)null);

        var act = async () => await CreateSut().Handle(new ApproveExpenseCommand { ExpenseId = Guid.NewGuid() }, default);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
