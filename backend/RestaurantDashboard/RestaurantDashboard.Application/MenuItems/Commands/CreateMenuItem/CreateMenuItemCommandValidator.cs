using FluentValidation;

namespace RestaurantDashboard.Application.MenuItems.Commands.CreateMenuItem;

public sealed class CreateMenuItemCommandValidator : AbstractValidator<CreateMenuItemCommand>
{
    public CreateMenuItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BasePrice).GreaterThan(0).WithMessage("Base price must be greater than zero.");
    }
}
