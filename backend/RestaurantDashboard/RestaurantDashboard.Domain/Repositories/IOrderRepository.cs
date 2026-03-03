using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;

namespace RestaurantDashboard.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetOpenOrdersAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByEmployeeAsync(Guid employeeId, DateOnly date, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    void Update(Order order);
}
