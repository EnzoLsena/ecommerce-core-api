using FluentValidation;

namespace Ecommerce.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}
