using MediatR;
using RestaurantDashboard.Application.MenuItems.Dtos;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.MenuItems.Queries.GetMenuItems;

public sealed class GetMenuItemsQueryHandler
    : IRequestHandler<GetMenuItemsQuery, IReadOnlyList<MenuItemDto>>
{
    private readonly IMenuItemRepository _menuItems;

    public GetMenuItemsQueryHandler(IMenuItemRepository menuItems) =>
        _menuItems = menuItems;

    public async Task<IReadOnlyList<MenuItemDto>> Handle(
        GetMenuItemsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _menuItems.GetAllAvailableAsync(cancellationToken);

        return items.Select(m => new MenuItemDto
        {
            Id = m.Id,
            Name = m.Name,
            Category = m.Category,
            Description = m.Description,
            BasePrice = m.BasePrice.Amount,
            IsAvailable = m.IsAvailable,
            StockQuantity = m.StockQuantity
        }).ToList();
    }
}
