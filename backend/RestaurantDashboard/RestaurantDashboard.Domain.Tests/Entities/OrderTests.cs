using FluentAssertions;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Enums;
using RestaurantDashboard.Domain.Exceptions;
using RestaurantDashboard.Domain.ValueObjects;

namespace RestaurantDashboard.Domain.Tests.Entities;

public sealed class OrderTests
{
    private static readonly Guid EmployeeId = Guid.NewGuid();

    [Fact]
    public void Open_WithValidData_CreatesOpenOrder()
    {
        var order = Order.Open(5, EmployeeId);

        order.TableNumber.Should().Be(5);
        order.EmployeeId.Should().Be(EmployeeId);
        order.Status.Should().Be(OrderStatus.Open);
        order.Items.Should().BeEmpty();
        order.DomainEvents.Should().ContainSingle();
    }

    [Fact]
    public void Open_WithZeroTableNumber_ThrowsDomainException()
    {
        var act = () => Order.Open(0, EmployeeId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddItem_ToOpenOrder_AddsItemToCollection()
    {
        var order = Order.Open(1, EmployeeId);
        var menuId = Guid.NewGuid();
        var price = Money.From(9.99m);

        order.AddItem(menuId, "Burger", 2, price);

        order.Items.Should().HaveCount(1);
        order.Subtotal.Amount.Should().Be(19.98m);
    }

    [Fact]
    public void AddItem_SameMenuItemTwice_MergesQuantity()
    {
        var order = Order.Open(1, EmployeeId);
        var menuId = Guid.NewGuid();
        var price = Money.From(10m);

        order.AddItem(menuId, "Pizza", 1, price);
        order.AddItem(menuId, "Pizza", 2, price);

        order.Items.Should().HaveCount(1);
        order.Items.First().Quantity.Should().Be(3);
    }

    [Fact]
    public void RemoveItem_ExistingItem_RemovesItFromCollection()
    {
        var order = Order.Open(1, EmployeeId);
        var menuId = Guid.NewGuid();
        order.AddItem(menuId, "Salad", 1, Money.From(5m));
        var itemId = order.Items.First().Id;

        order.RemoveItem(itemId);

        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void Close_OpenOrderWithItems_ReturnsSaleAndRaisesEvent()
    {
        var order = Order.Open(2, EmployeeId);
        order.AddItem(Guid.NewGuid(), "Steak", 1, Money.From(30m));
        order.ClearDomainEvents();

        var sale = order.Close(PaymentMethod.Cash, 5m);

        order.Status.Should().Be(OrderStatus.Closed);
        sale.Should().NotBeNull();
        sale.ProcessedByEmployeeId.Should().Be(EmployeeId);
        order.DomainEvents.Should().ContainSingle();
    }

    [Fact]
    public void Close_OrderWithNoItems_ThrowsDomainException()
    {
        var order = Order.Open(3, EmployeeId);

        var act = () => order.Close(PaymentMethod.Card, 0m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Void_OpenOrder_SetsStatusToVoided()
    {
        var order = Order.Open(4, EmployeeId);

        order.Void("Customer left");

        order.Status.Should().Be(OrderStatus.Voided);
    }

    [Fact]
    public void Void_ClosedOrder_ThrowsDomainException()
    {
        var order = Order.Open(5, EmployeeId);
        order.AddItem(Guid.NewGuid(), "Soup", 1, Money.From(7m));
        order.Close(PaymentMethod.Cash, 0m);

        var act = () => order.Void("Error");

        act.Should().Throw<DomainException>();
    }
}
