using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Sales.Commands.VoidOrder;

public sealed class VoidOrderCommandHandler : IRequestHandler<VoidOrderCommand, Unit>
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;
    private readonly IPublisher _publisher;

    public VoidOrderCommandHandler(IOrderRepository orders, IUnitOfWork uow, IPublisher publisher)
    {
        _orders = orders;
        _uow = uow;
        _publisher = publisher;
    }

    public async Task<Unit> Handle(VoidOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        order.Void(request.Reason);
        _orders.Update(order);
        await _uow.CommitAsync(cancellationToken);

        foreach (var domainEvent in order.DomainEvents)
            await _publisher.Publish(domainEvent, cancellationToken);

        order.ClearDomainEvents();
        return Unit.Value;
    }
}
