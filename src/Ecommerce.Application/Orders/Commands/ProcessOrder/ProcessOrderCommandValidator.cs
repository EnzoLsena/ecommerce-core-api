using FluentValidation;

namespace Ecommerce.Application.Orders.Commands.ProcessOrder;

public sealed class ProcessOrderCommandValidator : AbstractValidator<ProcessOrderCommand>
{
    public ProcessOrderCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}
