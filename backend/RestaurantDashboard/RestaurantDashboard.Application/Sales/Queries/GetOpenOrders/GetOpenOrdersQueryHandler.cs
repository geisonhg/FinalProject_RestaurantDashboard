using MediatR;
using RestaurantDashboard.Application.Sales.Dtos;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Sales.Queries.GetOpenOrders;

public sealed class GetOpenOrdersQueryHandler
    : IRequestHandler<GetOpenOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly IOrderRepository _orders;
    private readonly IEmployeeRepository _employees;

    public GetOpenOrdersQueryHandler(IOrderRepository orders, IEmployeeRepository employees)
    {
        _orders = orders;
        _employees = employees;
    }

    public async Task<IReadOnlyList<OrderDto>> Handle(
        GetOpenOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await _orders.GetOpenOrdersAsync(cancellationToken);

        var employeeIds = orders.Select(o => o.EmployeeId).Distinct().ToList();
        var allActive = await _employees.GetAllActiveAsync(cancellationToken);
        var employeeMap = allActive.ToDictionary(e => e.Id, e => e.FullName);

        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            TableNumber = o.TableNumber,
            EmployeeName = employeeMap.TryGetValue(o.EmployeeId, out var name) ? name : "Unknown",
            Status = o.Status.ToString(),
            OpenedAt = o.OpenedAt,
            ClosedAt = o.ClosedAt,
            Notes = o.Notes,
            Subtotal = o.Subtotal.Amount,
            Items = o.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                MenuItemId = i.MenuItemId,
                MenuItemName = i.MenuItemName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice.Amount,
                LineTotal = i.LineTotal.Amount
            }).ToList()
        }).ToList();
    }
}
