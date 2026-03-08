using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Domain.Repositories;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Sale?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
    Task<decimal> GetTotalRevenueAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
    Task<decimal> GetTotalTipsAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
    Task AddAsync(Sale sale, CancellationToken ct = default);
}
