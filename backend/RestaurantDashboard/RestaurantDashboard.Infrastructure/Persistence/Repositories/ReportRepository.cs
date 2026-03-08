using Microsoft.EntityFrameworkCore;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Infrastructure.Persistence.Repositories;

public sealed class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Report>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Reports
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync(ct);

    public Task<Report?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Reports.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task AddAsync(Report report, CancellationToken ct = default) =>
        await _context.Reports.AddAsync(report, ct);
}
