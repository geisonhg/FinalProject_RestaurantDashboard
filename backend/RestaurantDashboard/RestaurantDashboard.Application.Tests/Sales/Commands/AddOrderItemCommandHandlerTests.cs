using FluentAssertions;
using Moq;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Application.Sales.Commands.AddOrderItem;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Repositories;
using RestaurantDashboard.Domain.ValueObjects;

namespace RestaurantDashboard.Application.Tests.Sales.Commands;

public sealed class AddOrderItemCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _ordersMock = new();
    private readonly Mock<IMenuItemRepository> _menuItemsMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private AddOrderItemCommandHandler CreateSut() =>
        new(_ordersMock.Object, _menuItemsMock.Object, _uowMock.Object);

    private static Order CreateOpenOrder()
    {
        var order = Order.Open(1, Guid.NewGuid());
        order.ClearDomainEvents();
        return order;
    }

    private static MenuItem CreateMenuItem(bool isAvailable = true)
    {
        var item = MenuItem.Create("Burger", "Mains", 12m);
        if (!isAvailable) item.SetAvailability(false);
        return item;
    }

    [Fact]
    public async Task Handle_ValidRequest_AddsItemToOrderAndCommits()
    {
        var order = CreateOpenOrder();
        var menuItem = CreateMenuItem();

        _ordersMock.Setup(r => r.GetByIdWithItemsAsync(order.Id, default)).ReturnsAsync(order);
        _menuItemsMock.Setup(r => r.GetByIdAsync(menuItem.Id, default)).ReturnsAsync(menuItem);

        var command = new AddOrderItemCommand { OrderId = order.Id, MenuItemId = menuItem.Id, Quantity = 2 };
        await CreateSut().Handle(command, default);

        order.Items.Should().HaveCount(1);
        _uowMock.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ThrowsNotFoundException()
    {
        _ordersMock.Setup(r => r.GetByIdWithItemsAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Order?)null);

        var act = async () => await CreateSut().Handle(
            new AddOrderItemCommand { OrderId = Guid.NewGuid(), MenuItemId = Guid.NewGuid(), Quantity = 1 }, default);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_UnavailableMenuItem_ThrowsDomainException()
    {
        var order = CreateOpenOrder();
        var menuItem = CreateMenuItem(isAvailable: false);

        _ordersMock.Setup(r => r.GetByIdWithItemsAsync(order.Id, default)).ReturnsAsync(order);
        _menuItemsMock.Setup(r => r.GetByIdAsync(menuItem.Id, default)).ReturnsAsync(menuItem);

        var act = async () => await CreateSut().Handle(
            new AddOrderItemCommand { OrderId = order.Id, MenuItemId = menuItem.Id, Quantity = 1 }, default);

        await act.Should().ThrowAsync<Domain.Exceptions.DomainException>();
    }
}
