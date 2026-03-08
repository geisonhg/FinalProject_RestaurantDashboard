using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Domain.Repositories;

public interface IReportRepository
{
    Task<IReadOnlyList<Report>> GetAllAsync(CancellationToken ct = default);
    Task<Report?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Report report, CancellationToken ct = default);
}
