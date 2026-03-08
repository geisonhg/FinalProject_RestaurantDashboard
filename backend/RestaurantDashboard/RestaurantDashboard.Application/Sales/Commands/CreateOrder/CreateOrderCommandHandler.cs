using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Application.Sales.Dtos;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Sales.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orders;
    private readonly IEmployeeRepository _employees;
    private readonly IUnitOfWork _uow;
    private readonly IPublisher _publisher;

    public CreateOrderCommandHandler(
        IOrderRepository orders,
        IEmployeeRepository employees,
        IUnitOfWork uow,
        IPublisher publisher)
    {
        _orders = orders;
        _employees = employees;
        _uow = uow;
        _publisher = publisher;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(request.EmployeeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId);

        if (!employee.IsActive)
            throw new ForbiddenException($"Employee '{employee.FullName}' is inactive.");

        var order = Order.Open(request.TableNumber, request.EmployeeId, request.Notes);
        await _orders.AddAsync(order, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        foreach (var domainEvent in order.DomainEvents)
            await _publisher.Publish(domainEvent, cancellationToken);

        order.ClearDomainEvents();

        return new OrderDto
        {
            Id = order.Id,
            TableNumber = order.TableNumber,
            EmployeeName = employee.FullName,
            Status = order.Status.ToString(),
            OpenedAt = order.OpenedAt,
            Notes = order.Notes,
            Subtotal = 0m,
            Items = []
        };
    }
}
