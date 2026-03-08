using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Application.Sales.Dtos;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Sales.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IOrderRepository _orders;
    private readonly IEmployeeRepository _employees;

    public GetOrderByIdQueryHandler(IOrderRepository orders, IEmployeeRepository employees)
    {
        _orders = orders;
        _employees = employees;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        var employee = await _employees.GetByIdAsync(order.EmployeeId, cancellationToken);

        return new OrderDto
        {
            Id = order.Id,
            TableNumber = order.TableNumber,
            EmployeeName = employee?.FullName ?? "Unknown",
            Status = order.Status.ToString(),
            OpenedAt = order.OpenedAt,
            ClosedAt = order.ClosedAt,
            Notes = order.Notes,
            Subtotal = order.Subtotal.Amount,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                MenuItemId = i.MenuItemId,
                MenuItemName = i.MenuItemName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice.Amount,
                LineTotal = i.LineTotal.Amount
            }).ToList()
        };
    }
}
