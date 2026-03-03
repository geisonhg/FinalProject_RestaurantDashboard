using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;

namespace RestaurantDashboard.Domain.Repositories;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
    Task<IReadOnlyList<Expense>> GetByCategoryAsync(ExpenseCategory category, CancellationToken ct = default);
    Task<decimal> GetTotalAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
    Task AddAsync(Expense expense, CancellationToken ct = default);
    void Update(Expense expense);
}
