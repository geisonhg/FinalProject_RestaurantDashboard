using FluentAssertions;
using Moq;
using RestaurantDashboard.Application.MenuItems.Commands.CreateMenuItem;
using RestaurantDashboard.Domain.Entities;
using RestaurantDashboard.Domain.Repositories;

namespace RestaurantDashboard.Application.Tests.MenuItems.Commands;

public sealed class CreateMenuItemCommandHandlerTests
{
    private readonly Mock<IMenuItemRepository> _menuItemsMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private CreateMenuItemCommandHandler CreateSut() =>
        new(_menuItemsMock.Object, _uowMock.Object);

    [Fact]
    public async Task Handle_ValidCommand_CreatesMenuItemAndReturnsDto()
    {
        var command = new CreateMenuItemCommand { Name = "Fish & Chips", Category = "Mains", BasePrice = 14.50m };

        var result = await CreateSut().Handle(command, default);

        result.Should().NotBeNull();
        result.Name.Should().Be("Fish & Chips");
        result.Category.Should().Be("Mains");
        result.BasePrice.Should().Be(14.50m);
        result.IsAvailable.Should().BeTrue();
        _menuItemsMock.Verify(r => r.AddAsync(It.IsAny<MenuItem>(), default), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(default), Times.Once);
    }
}
