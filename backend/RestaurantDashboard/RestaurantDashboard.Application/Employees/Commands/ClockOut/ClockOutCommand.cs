using MediatR;

namespace RestaurantDashboard.Application.Employees.Commands.ClockOut;

public sealed record ClockOutCommand : IRequest<Unit>
{
    public Guid EmployeeId { get; init; }
    public Guid ShiftId { get; init; }
    public decimal TipsEarned { get; init; }
}
