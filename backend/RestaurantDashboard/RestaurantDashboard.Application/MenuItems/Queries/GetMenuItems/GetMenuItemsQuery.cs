using MediatR;
using RestaurantDashboard.Application.MenuItems.Dtos;

namespace RestaurantDashboard.Application.MenuItems.Queries.GetMenuItems;

public sealed record GetMenuItemsQuery : IRequest<IReadOnlyList<MenuItemDto>>;
