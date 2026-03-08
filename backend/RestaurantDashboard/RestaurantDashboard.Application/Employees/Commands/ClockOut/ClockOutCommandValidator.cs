using FluentValidation;

namespace RestaurantDashboard.Application.Employees.Commands.ClockOut;

public sealed class ClockOutCommandValidator : AbstractValidator<ClockOutCommand>
{
    public ClockOutCommandValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.ShiftId).NotEmpty();
        RuleFor(x => x.TipsEarned).GreaterThanOrEqualTo(0).WithMessage("Tips cannot be negative.");
    }
}
