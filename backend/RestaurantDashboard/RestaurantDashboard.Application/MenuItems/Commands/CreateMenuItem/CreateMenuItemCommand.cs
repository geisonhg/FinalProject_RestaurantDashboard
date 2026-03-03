using MediatR;
using RestaurantDashboard.Application.MenuItems.Dtos;

namespace RestaurantDashboard.Application.MenuItems.Commands.CreateMenuItem;

public sealed record CreateMenuItemCommand : IRequest<MenuItemDto>
{
    public string Name { get; init; } = default!;
    public string Category { get; init; } = default!;
    public decimal BasePrice { get; init; }
}
