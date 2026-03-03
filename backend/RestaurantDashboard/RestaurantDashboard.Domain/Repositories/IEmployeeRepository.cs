using RestaurantDashboard.Domain.Entities;

namespace RestaurantDashboard.Domain.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Employee?> GetByIdWithShiftsAsync(Guid id, CancellationToken ct = default);
    Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Employee>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(Employee employee, CancellationToken ct = default);
    void Update(Employee employee);
}
