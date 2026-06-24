using FluentValidation;

namespace Ecommerce.Application.Orders.Commands.DeleteOrder;

public sealed class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}
