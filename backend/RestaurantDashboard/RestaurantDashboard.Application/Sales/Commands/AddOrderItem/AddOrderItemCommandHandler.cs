using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Sales.Commands.AddOrderItem;

public sealed class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, Unit>
{
    private readonly IOrderRepository _orders;
    private readonly IMenuItemRepository _menuItems;
    private readonly IUnitOfWork _uow;

    public AddOrderItemCommandHandler(
        IOrderRepository orders,
        IMenuItemRepository menuItems,
        IUnitOfWork uow)
    {
        _orders = orders;
        _menuItems = menuItems;
        _uow = uow;
    }

    public async Task<Unit> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        var menuItem = await _menuItems.GetByIdAsync(request.MenuItemId, cancellationToken)
            ?? throw new NotFoundException(nameof(MenuItem), request.MenuItemId);

        if (!menuItem.IsAvailable)
            throw new Domain.Exceptions.DomainException($"Menu item '{menuItem.Name}' is not available.");

        order.AddItem(menuItem.Id, menuItem.Name, request.Quantity, menuItem.BasePrice);

        _orders.Update(order);
        await _uow.CommitAsync(cancellationToken);
        return Unit.Value;
    }
}
