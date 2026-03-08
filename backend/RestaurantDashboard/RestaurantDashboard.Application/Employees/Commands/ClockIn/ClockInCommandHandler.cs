using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Employees.Commands.ClockIn;

public sealed class ClockInCommandHandler : IRequestHandler<ClockInCommand, Guid>
{
    private readonly IEmployeeRepository _employees;
    private readonly IUnitOfWork _uow;

    public ClockInCommandHandler(IEmployeeRepository employees, IUnitOfWork uow)
    {
        _employees = employees;
        _uow = uow;
    }

    public async Task<Guid> Handle(ClockInCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdWithShiftsAsync(request.EmployeeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId);

        var shift = employee.ClockIn();

        _employees.Update(employee);
        await _uow.CommitAsync(cancellationToken);

        return shift.Id;
    }
}
