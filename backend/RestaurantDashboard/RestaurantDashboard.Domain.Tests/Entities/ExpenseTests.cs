using FluentAssertions;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Exceptions;

namespace RestaurantDashboard.Domain.Tests.Entities;

public sealed class ExpenseTests
{
    private static readonly Guid EmployeeId = Guid.NewGuid();

    [Fact]
    public void Record_WithValidData_CreatesUnapprovedExpense()
    {
        var expense = Expense.Record(
            ExpenseCategory.Food,
            150m,
            DateOnly.FromDateTime(DateTime.Today),
            "Weekly produce order",
            EmployeeId);

        expense.Amount.Amount.Should().Be(150m);
        expense.Category.Should().Be(ExpenseCategory.Food);
        expense.IsApproved.Should().BeFalse();
        expense.RecordedByEmployeeId.Should().Be(EmployeeId);
    }

    [Fact]
    public void Record_WithEmptyDescription_ThrowsDomainException()
    {
        var act = () => Expense.Record(
            ExpenseCategory.Utilities,
            50m,
            DateOnly.FromDateTime(DateTime.Today),
            string.Empty,
            EmployeeId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Approve_UnapprovedExpense_SetsIsApprovedTrue()
    {
        var expense = Expense.Record(ExpenseCategory.Food, 100m, DateOnly.FromDateTime(DateTime.Today), "Supplies", EmployeeId);

        expense.Approve();

        expense.IsApproved.Should().BeTrue();
    }

    [Fact]
    public void Revoke_ApprovedExpense_SetsIsApprovedFalse()
    {
        var expense = Expense.Record(ExpenseCategory.Food, 100m, DateOnly.FromDateTime(DateTime.Today), "Supplies", EmployeeId);
        expense.Approve();

        expense.Revoke();

        expense.IsApproved.Should().BeFalse();
    }
}
