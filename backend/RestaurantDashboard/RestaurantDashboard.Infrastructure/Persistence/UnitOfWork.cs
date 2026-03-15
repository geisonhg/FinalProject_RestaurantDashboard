using Microsoft.EntityFrameworkCore;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context) => _context = context;

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // In Blazor Server, DbContext lives for the entire circuit.
            // On failure, detach all pending entries so they don't get retried
            // on the next SaveChanges call (which belongs to a different operation).
            foreach (var entry in _context.ChangeTracker.Entries().ToList())
            {
                if (entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                    entry.State = EntityState.Detached;
            }
            throw;
        }
    }
}
