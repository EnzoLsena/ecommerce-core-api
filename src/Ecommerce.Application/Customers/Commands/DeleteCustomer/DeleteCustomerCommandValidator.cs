using FluentValidation;

namespace Ecommerce.Application.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}
