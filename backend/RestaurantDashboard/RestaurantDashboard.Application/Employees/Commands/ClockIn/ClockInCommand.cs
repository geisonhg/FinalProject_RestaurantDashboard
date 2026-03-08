using MediatR;

namespace RestaurantDashboard.Application.Employees.Commands.ClockIn;

public sealed record ClockInCommand : IRequest<Guid>
{
    public Guid EmployeeId { get; init; }
}
