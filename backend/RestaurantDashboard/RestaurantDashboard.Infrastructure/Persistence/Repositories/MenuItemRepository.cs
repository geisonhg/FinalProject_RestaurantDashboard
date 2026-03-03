using Microsoft.EntityFrameworkCore;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Infrastructure.Persistence.Repositories;

public sealed class MenuItemRepository : IMenuItemRepository
{
    private readonly AppDbContext _context;

    public MenuItemRepository(AppDbContext context) => _context = context;

    public Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _context.MenuItems.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<IReadOnlyList<MenuItem>> GetAllAvailableAsync(CancellationToken ct) =>
        await _context.MenuItems
            .Where(m => m.IsAvailable)
            .OrderBy(m => m.Category)
            .ThenBy(m => m.Name)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(string category, CancellationToken ct) =>
        await _context.MenuItems
            .Where(m => m.Category == category && m.IsAvailable)
            .ToListAsync(ct);

    public async Task AddAsync(MenuItem menuItem, CancellationToken ct) =>
        await _context.MenuItems.AddAsync(menuItem, ct);

    public void Update(MenuItem menuItem) =>
        _context.MenuItems.Update(menuItem);
}
