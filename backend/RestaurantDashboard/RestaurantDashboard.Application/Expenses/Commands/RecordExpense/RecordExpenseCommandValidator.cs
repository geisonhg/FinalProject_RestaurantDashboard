using FluentValidation;

namespace RestaurantDashboard.Application.Expenses.Commands.RecordExpense;

public sealed class RecordExpenseCommandValidator : AbstractValidator<RecordExpenseCommand>
{
    public RecordExpenseCommandValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Expense date cannot be in the future.");
        RuleFor(x => x.RecordedByEmployeeId).NotEmpty();
    }
}
