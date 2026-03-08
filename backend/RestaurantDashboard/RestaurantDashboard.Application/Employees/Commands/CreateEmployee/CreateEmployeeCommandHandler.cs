using MediatR;
using RestaurantDashboard.Application.Employees.Dtos;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Employees.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandHandler
    : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _employees;
    private readonly IUnitOfWork _uow;

    public CreateEmployeeCommandHandler(IEmployeeRepository employees, IUnitOfWork uow)
    {
        _employees = employees;
        _uow = uow;
    }

    public async Task<EmployeeDto> Handle(
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee = Employee.Create(
            request.FirstName,
            request.LastName,
            request.Role,
            request.HireDate);

        await _employees.AddAsync(employee, cancellationToken);
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
