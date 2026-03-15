using Microsoft.EntityFrameworkCore;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Infrastructure.Persistence.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context) => _context = context;

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _context.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<Employee?> GetByIdWithShiftsAsync(Guid id, CancellationToken ct) =>
        _context.Employees
            .Include(e => e.Shifts)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken ct) =>
        _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId, ct);

    public async Task<IReadOnlyList<Employee>> GetAllActiveAsync(CancellationToken ct) =>
        await _context.Employees
            .AsNoTracking()
            .Where(e => e.IsActive)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Employee>> GetAllActiveWithShiftsAsync(CancellationToken ct) =>
        await _context.Employees
            .Include(e => e.Shifts)
            .AsNoTracking()
            .Where(e => e.IsActive)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(ct);

    public async Task AddAsync(Employee employee, CancellationToken ct) =>
        await _context.Employees.AddAsync(employee, ct);

    public void Update(Employee employee)
    {
        _context.ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            var isTracked = _context.ChangeTracker.Entries<Employee>()
                .Any(e => e.Entity.Id == employee.Id);

            if (!isTracked)
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
                _context.Employees.Update(employee);
                return;
            }

            var trackedShiftIds = _context.ChangeTracker.Entries<Shift>()
                .Select(e => e.Entity.Id)
                .ToHashSet();

            foreach (var shift in employee.Shifts)
            {
                if (!trackedShiftIds.Contains(shift.Id))
                    _context.Add(shift);
            }
        }
        finally
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    public void Remove(Employee employee) =>
        _context.Employees.Remove(employee);
}
