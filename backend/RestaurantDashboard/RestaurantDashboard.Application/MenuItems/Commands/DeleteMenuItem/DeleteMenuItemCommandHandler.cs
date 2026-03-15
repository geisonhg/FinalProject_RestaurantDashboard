using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.MenuItems.Commands.DeleteMenuItem;

public sealed class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand>
{
    private readonly IMenuItemRepository _menuItems;
    private readonly IUnitOfWork _uow;

    public DeleteMenuItemCommandHandler(IMenuItemRepository menuItems, IUnitOfWork uow)
    {
        _menuItems = menuItems;
        _uow = uow;
    }

    public async Task Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItems.GetByIdAsync(request.MenuItemId, cancellationToken)
            ?? throw new NotFoundException(nameof(MenuItem), request.MenuItemId);

        _menuItems.Remove(menuItem);
        await _uow.CommitAsync(cancellationToken);
    }
}
