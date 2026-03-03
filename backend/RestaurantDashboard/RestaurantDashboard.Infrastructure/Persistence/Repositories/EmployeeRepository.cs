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
            .Where(e => e.IsActive)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(ct);

    public async Task AddAsync(Employee employee, CancellationToken ct) =>
        await _context.Employees.AddAsync(employee, ct);

    public void Update(Employee employee) =>
        _context.Employees.Update(employee);
}
