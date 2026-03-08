using RestaurantDashboard.Domain.Common;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Exceptions;

namespace RestaurantDashboard.Domain.Entities;

public sealed class Shift : Entity
{
    public Guid EmployeeId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly ClockIn { get; private set; }
    public TimeOnly? ClockOut { get; private set; }
    public decimal TipsEarned { get; private set; }
    public ShiftStatus Status { get; private set; }

    public TimeSpan Duration => ClockOut.HasValue
        ? ClockOut.Value - ClockIn
        : TimeSpan.Zero;

    private Shift() { }

    internal static Shift StartNow(Guid employeeId)
    {
        var now = DateTime.UtcNow;
        return new Shift
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            Date = DateOnly.FromDateTime(now),
            ClockIn = TimeOnly.FromDateTime(now),
            Status = ShiftStatus.Active
        };
    }

    internal void Close(TimeOnly clockOut, decimal tipsEarned)
    {
        if (Status != ShiftStatus.Active)
            throw new DomainException($"Cannot close a shift with status '{Status}'.");

        if (clockOut < ClockIn)
            throw new DomainException("Clock-out time cannot be before clock-in time.");

        Guard.AgainstNegative(tipsEarned, nameof(tipsEarned));

        ClockOut = clockOut;
        TipsEarned = tipsEarned;
        Status = ShiftStatus.Completed;
    }
}
