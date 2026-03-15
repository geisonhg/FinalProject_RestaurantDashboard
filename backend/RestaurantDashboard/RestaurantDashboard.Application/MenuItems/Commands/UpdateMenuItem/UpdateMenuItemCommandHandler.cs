using MediatR;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Application.MenuItems.Dtos;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.MenuItems.Commands.UpdateMenuItem;

public sealed class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, MenuItemDto>
{
    private readonly IMenuItemRepository _menuItems;
    private readonly IUnitOfWork _uow;

    public UpdateMenuItemCommandHandler(IMenuItemRepository menuItems, IUnitOfWork uow)
    {
        _menuItems = menuItems;
        _uow = uow;
    }

    public async Task<MenuItemDto> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItems.GetByIdAsync(request.MenuItemId, cancellationToken)
            ?? throw new NotFoundException(nameof(MenuItem), request.MenuItemId);

        menuItem.Update(request.Name, request.Category, request.Description);
        menuItem.UpdatePrice(request.BasePrice);
        menuItem.SetAvailability(request.IsAvailable);

        _menuItems.Update(menuItem);
        await _uow.CommitAsync(cancellationToken);

        return new MenuItemDto
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Category = menuItem.Category,
            Description = menuItem.Description,
            BasePrice = menuItem.BasePrice.Amount,
            IsAvailable = menuItem.IsAvailable
        };
    }
}
