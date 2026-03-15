using Microsoft.EntityFrameworkCore;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Infrastructure.Persistence.Repositories;

public sealed class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _context;

    public SaleRepository(AppDbContext context) => _context = context;

    public Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _context.Sales.Include(s => s.Tip).FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<Sale?> GetByOrderIdAsync(Guid orderId, CancellationToken ct) =>
        _context.Sales.FirstOrDefaultAsync(s => s.OrderId == orderId, ct);

    public async Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken ct) =>
        await _context.Sales
            .Include(s => s.Tip)
            .AsNoTracking()
            .Where(s => s.Date >= from && s.Date <= to)
            .OrderByDescending(s => s.Date)
            .ToListAsync(ct);

    public Task<decimal> GetTotalRevenueAsync(DateOnly from, DateOnly to, CancellationToken ct) =>
        _context.Sales
            .Where(s => s.Date >= from && s.Date <= to)
            .SumAsync(s => s.TotalAmount.Amount, ct);

    public Task<decimal> GetTotalTipsAsync(DateOnly from, DateOnly to, CancellationToken ct) =>
        _context.Sales
            .Where(s => s.Date >= from && s.Date <= to)
            .SumAsync(s => s.TipAmount.Amount, ct);

    public async Task AddAsync(Sale sale, CancellationToken ct) =>
        await _context.Sales.AddAsync(sale, ct);

    public async Task<Dictionary<string, decimal>> GetTopSellingItemsAsync(DateOnly from, DateOnly to, CancellationToken ct) =>
        await _context.Sales
            .Where(s => s.Date >= from && s.Date <= to)
            .Join(_context.OrderItems,
                sale => sale.OrderId,
                item => item.OrderId,
                (sale, item) => item)
            .GroupBy(i => i.MenuItemName)
            .Select(g => new { Name = g.Key, Revenue = g.Sum(i => i.UnitPrice.Amount * i.Quantity) })
            .OrderByDescending(x => x.Revenue)
            .Take(10)
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Name, x => x.Revenue, ct);
}
