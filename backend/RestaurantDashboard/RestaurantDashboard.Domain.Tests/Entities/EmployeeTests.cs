using FluentAssertions;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Exceptions;

namespace RestaurantDashboard.Domain.Tests.Entities;

public sealed class EmployeeTests
{
    private static Employee CreateEmployee() =>
        Employee.Create("John", "Doe", EmployeeRole.Waiter, DateOnly.FromDateTime(DateTime.Today));

    [Fact]
    public void Create_WithValidData_CreatesActiveEmployee()
    {
        var employee = Employee.Create("Jane", "Smith", EmployeeRole.Manager, DateOnly.FromDateTime(DateTime.Today));

        employee.FirstName.Should().Be("Jane");
        employee.LastName.Should().Be("Smith");
        employee.FullName.Should().Be("Jane Smith");
        employee.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ClockIn_ActiveEmployee_ReturnsShiftAndAddsToShifts()
    {
        var employee = CreateEmployee();

        var shift = employee.ClockIn();

        shift.Should().NotBeNull();
        shift.EmployeeId.Should().Be(employee.Id);
        employee.Shifts.Should().HaveCount(1);
    }

    [Fact]
    public void ClockIn_InactiveEmployee_ThrowsDomainException()
    {
        var employee = CreateEmployee();
        employee.Deactivate();

        var act = () => employee.ClockIn();

        act.Should().Throw<DomainException>().WithMessage("*inactive*");
    }

    [Fact]
    public void ClockIn_AlreadyClockedIn_ThrowsDomainException()
    {
        var employee = CreateEmployee();
        employee.ClockIn();

        var act = () => employee.ClockIn();

        act.Should().Throw<DomainException>().WithMessage("*already has an active shift*");
    }

    [Fact]
    public void ClockOut_ActiveShift_CompletesShift()
    {
        var employee = CreateEmployee();
        var shift = employee.ClockIn();

        employee.ClockOut(shift.Id, 20m);

        employee.Shifts.Single().TipsEarned.Should().Be(20m);
    }

    [Fact]
    public void Deactivate_ActiveEmployee_SetsIsActiveFalse()
    {
        var employee = CreateEmployee();

        employee.Deactivate();

        employee.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_InactiveEmployee_SetsIsActiveTrue()
    {
        var employee = CreateEmployee();
        employee.Deactivate();

        employee.Activate();

        employee.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateDetails_WithNewValues_UpdatesEmployeeProperties()
    {
        var employee = CreateEmployee();

        employee.UpdateDetails("Alice", "Wonder", EmployeeRole.Manager);

        employee.FirstName.Should().Be("Alice");
        employee.LastName.Should().Be("Wonder");
        employee.Role.Should().Be(EmployeeRole.Manager);
    }
}
