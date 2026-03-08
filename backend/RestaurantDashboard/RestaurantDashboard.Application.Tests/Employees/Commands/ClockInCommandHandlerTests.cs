using FluentAssertions;
using Moq;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Application.Employees.Commands.ClockIn;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Tests.Employees.Commands;

public sealed class ClockInCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeesMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private ClockInCommandHandler CreateSut() =>
        new(_employeesMock.Object, _uowMock.Object);

    [Fact]
    public async Task Handle_ActiveEmployee_ClocksInAndReturnsShiftId()
    {
        var employee = Employee.Create("Sara", "Lane", EmployeeRole.Waiter, DateOnly.FromDateTime(DateTime.Today));
        _employeesMock.Setup(r => r.GetByIdWithShiftsAsync(employee.Id, default)).ReturnsAsync(employee);

        var result = await CreateSut().Handle(new ClockInCommand { EmployeeId = employee.Id }, default);

        result.Should().NotBeEmpty();
        employee.Shifts.Should().HaveCount(1);
        _uowMock.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_EmployeeNotFound_ThrowsNotFoundException()
    {
        _employeesMock.Setup(r => r.GetByIdWithShiftsAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Employee?)null);

        var act = async () => await CreateSut().Handle(new ClockInCommand { EmployeeId = Guid.NewGuid() }, default);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
