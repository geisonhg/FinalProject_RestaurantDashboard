using MediatR;
using RestaurantDashboard.Application.Employees.Dtos;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Employees.Queries.GetAllEmployees;

public sealed class GetAllEmployeesQueryHandler
    : IRequestHandler<GetAllEmployeesQuery, IReadOnlyList<EmployeeDto>>
{
    private readonly IEmployeeRepository _employees;

    public GetAllEmployeesQueryHandler(IEmployeeRepository employees) =>
        _employees = employees;

    public async Task<IReadOnlyList<EmployeeDto>> Handle(
        GetAllEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var employees = await _employees.GetAllActiveAsync(cancellationToken);

        return employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            FullName = e.FullName,
            Role = e.Role.ToString(),
            HireDate = e.HireDate,
            IsActive = e.IsActive
        }).ToList();
    }
}
