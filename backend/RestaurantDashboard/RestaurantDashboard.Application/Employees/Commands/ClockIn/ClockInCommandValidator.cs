using FluentValidation;

namespace RestaurantDashboard.Application.Employees.Commands.ClockIn;

public sealed class ClockInCommandValidator : AbstractValidator<ClockInCommand>
{
    public ClockInCommandValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
    }
}
