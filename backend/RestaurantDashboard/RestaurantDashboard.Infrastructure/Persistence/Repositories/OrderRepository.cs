using Microsoft.EntityFrameworkCore;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context) => _context = context;

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct) =>
        _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Order>> GetOpenOrdersAsync(CancellationToken ct) =>
        await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Open)
            .OrderBy(o => o.OpenedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct) =>
        await _context.Orders
            .AsNoTracking()
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.OpenedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Order>> GetByEmployeeAsync(Guid employeeId, DateOnly date, CancellationToken ct) =>
        await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .Where(o => o.EmployeeId == employeeId
                && o.OpenedAt.Date == date.ToDateTime(TimeOnly.MinValue).Date)
            .ToListAsync(ct);

    public async Task AddAsync(Order order, CancellationToken ct) =>
        await _context.Orders.AddAsync(order, ct);

    public void Update(Order order) =>
        _context.Orders.Update(order);
}
