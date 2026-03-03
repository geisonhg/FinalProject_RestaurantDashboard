using MediatR;
using RestaurantDashboard.Application.MenuItems.Dtos;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.MenuItems.Commands.CreateMenuItem;

public sealed class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, MenuItemDto>
{
    private readonly IMenuItemRepository _menuItems;
    private readonly IUnitOfWork _uow;

    public CreateMenuItemCommandHandler(IMenuItemRepository menuItems, IUnitOfWork uow)
    {
        _menuItems = menuItems;
        _uow = uow;
    }

    public async Task<MenuItemDto> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItem = MenuItem.Create(request.Name, request.Category, request.BasePrice);

        await _menuItems.AddAsync(menuItem, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        return new MenuItemDto
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Category = menuItem.Category,
            BasePrice = menuItem.BasePrice.Amount,
            IsAvailable = menuItem.IsAvailable
        };
    }
}
