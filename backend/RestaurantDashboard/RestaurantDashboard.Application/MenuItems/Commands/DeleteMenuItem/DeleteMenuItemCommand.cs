using MediatR;

namespace RestaurantDashboard.Application.MenuItems.Commands.DeleteMenuItem;

public sealed record DeleteMenuItemCommand(Guid MenuItemId) : IRequest;
