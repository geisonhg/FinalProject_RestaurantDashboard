using Microsoft.EntityFrameworkCore;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Infrastructure.Persistence.Repositories;

public sealed class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _context;

    public ExpenseRepository(AppDbContext context) => _context = context;

    public Task<Expense?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _context.Expenses.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken ct) =>
        await _context.Expenses
            .AsNoTracking()
            .Where(e => e.Date >= from && e.Date <= to)
            .OrderByDescending(e => e.Date)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Expense>> GetByCategoryAsync(ExpenseCategory category, CancellationToken ct) =>
        await _context.Expenses
            .AsNoTracking()
            .Where(e => e.Category == category)
            .OrderByDescending(e => e.Date)
            .ToListAsync(ct);

    public Task<decimal> GetTotalAsync(DateOnly from, DateOnly to, CancellationToken ct) =>
        _context.Expenses
            .Where(e => e.Date >= from && e.Date <= to && e.IsApproved)
            .SumAsync(e => e.Amount.Amount, ct);

    public async Task AddAsync(Expense expense, CancellationToken ct) =>
        await _context.Expenses.AddAsync(expense, ct);

    public void Update(Expense expense) =>
        _context.Expenses.Update(expense);

    public void Remove(Expense expense) =>
        _context.Expenses.Remove(expense);
}
