using MediatR;
using RestaurantDashboard.Application.Sales.Dtos;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Sales.Queries.GetClosedOrders;

public sealed class GetClosedOrdersQueryHandler
    : IRequestHandler<GetClosedOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly IOrderRepository _orders;
    private readonly IEmployeeRepository _employees;
    private readonly ISaleRepository _sales;

    public GetClosedOrdersQueryHandler(
        IOrderRepository orders,
        IEmployeeRepository employees,
        ISaleRepository sales)
    {
        _orders = orders;
        _employees = employees;
        _sales = sales;
    }

    public async Task<IReadOnlyList<OrderDto>> Handle(
        GetClosedOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var allClosed = await _orders.GetByStatusAsync(OrderStatus.Closed, cancellationToken);

        var fromDt = request.From.ToDateTime(TimeOnly.MinValue);
        var toDt = request.To.ToDateTime(TimeOnly.MaxValue);

        var filtered = allClosed
            .Where(o => o.ClosedAt >= fromDt && o.ClosedAt <= toDt)
            .OrderByDescending(o => o.ClosedAt)
            .ToList();

        var allEmployees = await _employees.GetAllActiveAsync(cancellationToken);
        var employeeMap = allEmployees.ToDictionary(e => e.Id, e => e.FullName);

        var salesInRange = await _sales.GetByDateRangeAsync(request.From, request.To, cancellationToken);
        var saleByOrder = salesInRange.ToDictionary(s => s.OrderId);

        return filtered.Select(o =>
        {
            saleByOrder.TryGetValue(o.Id, out var sale);
            return new OrderDto
            {
                Id = o.Id,
                TableNumber = o.TableNumber,
                EmployeeName = employeeMap.TryGetValue(o.EmployeeId, out var name) ? name : "Unknown",
                Status = o.Status.ToString(),
                OpenedAt = o.OpenedAt,
                ClosedAt = o.ClosedAt,
                Notes = o.Notes,
                Subtotal = o.Subtotal.Amount,
                DiscountAmount = o.DiscountAmount,
                TipAmount = sale?.TipAmount.Amount ?? 0m,
                PaymentMethod = sale?.PaymentMethod.ToString(),
                Items = o.Items
                    .Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        MenuItemId = i.MenuItemId,
                        MenuItemName = i.MenuItemName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice.Amount,
                        LineTotal = i.LineTotal.Amount
                    })
                    .ToList()
            };
        }).ToList();
    }
}
