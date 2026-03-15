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
            .Include(o => o.Items)
            .AsNoTracking()
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.ClosedAt)
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

    public void Update(Order order)
    {
        // Both ChangeTracker.Entries<T>() and _context.Entry(entity) trigger DetectChanges
        // when AutoDetectChangesEnabled is true. DetectChanges finds new OrderItems (which
        // have non-default Guid keys) and marks them Modified instead of Added, causing EF
        // to generate UPDATE for rows that don't exist yet → 0 rows → DbUpdateConcurrencyException.
        // Disable AutoDetectChanges while we inspect/fix states, then restore it.

        _context.ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            var orderIsTracked = _context.ChangeTracker.Entries<Order>()
                .Any(e => e.Entity.Id == order.Id);

            if (!orderIsTracked)
            {
                // Order not in context yet; re-enable so Update() graph traversal works normally.
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
                _context.Orders.Update(order);
                return;
            }

            // Order is already tracked. Collect IDs of already-tracked OrderItems.
            var trackedItemIds = _context.ChangeTracker.Entries<OrderItem>()
                .Select(e => e.Entity.Id)
                .ToHashSet();

            // Explicitly Add new items. _context.Add() uses EntryWithoutDetectChanges
            // internally (no DetectChanges triggered) AND properly traverses the entity
            // graph, so owned entities like UnitPrice are tracked correctly.
            foreach (var item in order.Items)
            {
                if (!trackedItemIds.Contains(item.Id))
                    _context.Add(item);
            }
        }
        finally
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
}
