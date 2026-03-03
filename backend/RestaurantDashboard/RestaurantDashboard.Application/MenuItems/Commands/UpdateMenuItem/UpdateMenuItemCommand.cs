using MediatR;
using RestaurantDashboard.Application.MenuItems.Dtos;

namespace RestaurantDashboard.Application.MenuItems.Commands.UpdateMenuItem;

public sealed record UpdateMenuItemCommand : IRequest<MenuItemDto>
{
    public Guid MenuItemId { get; init; }
    public string Name { get; init; } = default!;
    public string Category { get; init; } = default!;
    public decimal BasePrice { get; init; }
    public bool IsAvailable { get; init; }
}
