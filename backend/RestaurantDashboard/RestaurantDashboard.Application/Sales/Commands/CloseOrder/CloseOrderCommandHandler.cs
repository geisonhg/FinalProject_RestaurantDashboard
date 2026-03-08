using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Sales.Commands.CloseOrder;

public sealed class CloseOrderCommandHandler : IRequestHandler<CloseOrderCommand, Guid>
{
    private readonly IOrderRepository _orders;
    private readonly ISaleRepository _sales;
    private readonly IUnitOfWork _uow;
    private readonly IPublisher _publisher;

    public CloseOrderCommandHandler(
        IOrderRepository orders,
        ISaleRepository sales,
        IUnitOfWork uow,
        IPublisher publisher)
    {
        _orders = orders;
        _sales = sales;
        _uow = uow;
        _publisher = publisher;
    }

    public async Task<Guid> Handle(CloseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        var sale = order.Close(request.PaymentMethod, request.TipAmount);

        await _sales.AddAsync(sale, cancellationToken);
        _orders.Update(order);
        await _uow.CommitAsync(cancellationToken);

        foreach (var domainEvent in order.DomainEvents)
            await _publisher.Publish(domainEvent, cancellationToken);

        order.ClearDomainEvents();
        return sale.Id;
    }
}
