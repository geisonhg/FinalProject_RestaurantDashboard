using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Domain.Repositories;

public interface IMenuItemRepository
{
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<MenuItem>> GetAllAvailableAsync(CancellationToken ct = default);
    Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task AddAsync(MenuItem menuItem, CancellationToken ct = default);
    void Update(MenuItem menuItem);
}
