using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Application.Employees.Dtos;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Employees.Commands.UpdateEmployee;

public sealed class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _employees;
    private readonly IUnitOfWork _uow;

    public UpdateEmployeeCommandHandler(IEmployeeRepository employees, IUnitOfWork uow)
    {
        _employees = employees;
        _uow = uow;
    }

    public async Task<EmployeeDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId);

        employee.UpdateDetails(request.FirstName, request.LastName, request.Role);

        if (request.IsActive && !employee.IsActive)
            employee.Activate();
        else if (!request.IsActive && employee.IsActive)
            employee.Deactivate();

        _employees.Update(employee);
        await _uow.CommitAsync(cancellationToken);

        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            FullName = employee.FullName,
            Role = employee.Role.ToString(),
            HireDate = employee.HireDate,
            IsActive = employee.IsActive
        };
    }
}
