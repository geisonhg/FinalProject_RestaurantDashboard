using RestaurantDashboard.Domain.Common;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Exceptions;

namespace RestaurantDashboard.Domain.Entities;

public sealed class Employee : AggregateRoot
{
    private readonly List<Shift> _shifts = new();

    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string FullName => $"{FirstName} {LastName}";
    public EmployeeRole Role { get; private set; }
    public DateOnly HireDate { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? UserId { get; private set; }  // Link to ASP.NET Identity user

    public IReadOnlyCollection<Shift> Shifts => _shifts.AsReadOnly();

    private Employee() { }

    public static Employee Create(
        string firstName,
        string lastName,
        EmployeeRole role,
        DateOnly hireDate)
    {
        Guard.AgainstNullOrEmpty(firstName, nameof(firstName));
        Guard.AgainstNullOrEmpty(lastName, nameof(lastName));

        return new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            HireDate = hireDate,
            IsActive = true
        };
    }

    public void LinkToUser(Guid userId) => UserId = userId;

    public void UpdateDetails(string firstName, string lastName, EmployeeRole role)
    {
        Guard.AgainstNullOrEmpty(firstName, nameof(firstName));
        Guard.AgainstNullOrEmpty(lastName, nameof(lastName));
        FirstName = firstName;
        LastName = lastName;
        Role = role;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public Shift ClockIn()
    {
        if (!IsActive)
            throw new DomainException($"Employee '{FullName}' is inactive and cannot clock in.");

        if (_shifts.Any(s => s.Status == ShiftStatus.Active))
            throw new DomainException($"Employee '{FullName}' already has an active shift.");

        var shift = Shift.StartNow(Id);
        _shifts.Add(shift);
        return shift;
    }

    public void ClockOut(Guid shiftId, decimal tipsEarned)
    {
        var shift = _shifts.FirstOrDefault(s => s.Id == shiftId)
            ?? throw new DomainException($"Shift '{shiftId}' not found for employee '{FullName}'.");

        shift.Close(TimeOnly.FromDateTime(DateTime.UtcNow), tipsEarned);
    }

    public decimal GetTotalTips(DateOnly from, DateOnly to) =>
        _shifts
            .Where(s => s.Date >= from && s.Date <= to && s.Status == ShiftStatus.Completed)
            .Sum(s => s.TipsEarned);

    public double GetTotalHours(DateOnly from, DateOnly to) =>
        _shifts
            .Where(s => s.Date >= from && s.Date <= to && s.Status == ShiftStatus.Completed)
            .Sum(s => s.Duration.TotalHours);

    public int GetTotalShifts(DateOnly from, DateOnly to) =>
        _shifts.Count(s => s.Date >= from && s.Date <= to && s.Status == ShiftStatus.Completed);
}
