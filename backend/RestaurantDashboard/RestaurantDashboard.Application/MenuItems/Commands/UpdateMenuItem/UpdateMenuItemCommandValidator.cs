using FluentValidation;

namespace RestaurantDashboard.Application.MenuItems.Commands.UpdateMenuItem;

public sealed class UpdateMenuItemCommandValidator : AbstractValidator<UpdateMenuItemCommand>
{
    public UpdateMenuItemCommandValidator()
    {
        RuleFor(x => x.MenuItemId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BasePrice).GreaterThan(0).WithMessage("Base price must be greater than zero.");
    }
}
