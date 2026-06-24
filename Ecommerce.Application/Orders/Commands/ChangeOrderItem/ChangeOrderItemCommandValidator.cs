using FluentValidation;

namespace Ecommerce.Application.Orders.Commands.ChangeOrderItem;

public sealed class ChangeOrderItemCommandValidator : AbstractValidator<ChangeOrderItemCommand>
{
    public ChangeOrderItemCommandValidator()
    {
        RuleFor(command => command.OrderId).NotEmpty();
        RuleFor(command => command.ProductId).NotEmpty();
        RuleFor(command => command.Quantity).GreaterThan(0);
        RuleFor(command => command.UnitPrice).GreaterThanOrEqualTo(0);
    }
}
