using FluentValidation;

namespace Ecommerce.Application.Orders.Commands.UpdateOrderCustomer;

public sealed class UpdateOrderCustomerCommandValidator : AbstractValidator<UpdateOrderCustomerCommand>
{
    public UpdateOrderCustomerCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.CustomerId).NotEmpty();
    }
}
