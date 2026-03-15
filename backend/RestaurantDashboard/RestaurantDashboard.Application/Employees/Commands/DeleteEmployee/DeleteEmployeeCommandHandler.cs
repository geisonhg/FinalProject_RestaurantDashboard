using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Employees.Commands.DeleteEmployee;

public sealed class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IEmployeeRepository _employees;
    private readonly IUnitOfWork _uow;

    public DeleteEmployeeCommandHandler(IEmployeeRepository employees, IUnitOfWork uow)
    {
        _employees = employees;
        _uow = uow;
    }

    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId);

        employee.Deactivate();
        _employees.Update(employee);
        await _uow.CommitAsync(cancellationToken);
    }
}
