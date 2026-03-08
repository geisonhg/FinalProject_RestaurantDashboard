using FluentAssertions;
using MediatR;
using Moq;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Application.Sales.Commands.CreateOrder;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Tests.Sales.Commands;

public sealed class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _ordersMock = new();
    private readonly Mock<IEmployeeRepository> _employeesMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IPublisher> _publisherMock = new();

    private CreateOrderCommandHandler CreateSut() =>
        new(_ordersMock.Object, _employeesMock.Object, _uowMock.Object, _publisherMock.Object);

    [Fact]
    public async Task Handle_ActiveEmployee_CreatesOrderAndReturnsDto()
    {
        var employee = Employee.Create("Tom", "Jones", EmployeeRole.Waiter, DateOnly.FromDateTime(DateTime.Today));
        _employeesMock.Setup(r => r.GetByIdAsync(employee.Id, default)).ReturnsAsync(employee);

        var command = new CreateOrderCommand { TableNumber = 3, EmployeeId = employee.Id };
        var result = await CreateSut().Handle(command, default);

        result.TableNumber.Should().Be(3);
        result.EmployeeName.Should().Be("Tom Jones");
        _ordersMock.Verify(r => r.AddAsync(It.IsAny<Order>(), default), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_EmployeeNotFound_ThrowsNotFoundException()
    {
        _employeesMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Employee?)null);

        var act = async () => await CreateSut().Handle(new CreateOrderCommand { TableNumber = 1, EmployeeId = Guid.NewGuid() }, default);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_InactiveEmployee_ThrowsForbiddenException()
    {
        var employee = Employee.Create("Bob", "Lee", EmployeeRole.Waiter, DateOnly.FromDateTime(DateTime.Today));
        employee.Deactivate();
        _employeesMock.Setup(r => r.GetByIdAsync(employee.Id, default)).ReturnsAsync(employee);

        var act = async () => await CreateSut().Handle(new CreateOrderCommand { TableNumber = 1, EmployeeId = employee.Id }, default);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
