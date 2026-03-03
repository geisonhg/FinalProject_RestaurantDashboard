using FluentAssertions;
using MediatR;
using Moq;
using RestaurantDashboard.Application.Common.Exceptions;
using RestaurantDashboard.Application.Sales.Commands.VoidOrder;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Tests.Sales.Commands;

public sealed class VoidOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _ordersMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private VoidOrderCommandHandler CreateSut() =>
        new(_ordersMock.Object, _uowMock.Object);

    [Fact]
    public async Task Handle_OpenOrder_VoidsOrderAndReturnsUnit()
    {
        var order = Order.Open(1, Guid.NewGuid());
        _ordersMock.Setup(r => r.GetByIdAsync(order.Id, default)).ReturnsAsync(order);

        var result = await CreateSut().Handle(new VoidOrderCommand { OrderId = order.Id, Reason = "Customer cancelled" }, default);

        result.Should().Be(Unit.Value);
        order.Status.Should().Be(Domain.Enums.OrderStatus.Voided);
        _uowMock.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ThrowsNotFoundException()
    {
        _ordersMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Order?)null);

        var act = async () => await CreateSut().Handle(
            new VoidOrderCommand { OrderId = Guid.NewGuid(), Reason = "Test" }, default);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
