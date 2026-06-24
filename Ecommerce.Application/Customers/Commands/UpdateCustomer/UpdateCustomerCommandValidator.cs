using FluentValidation;

namespace Ecommerce.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress();
    }
}
