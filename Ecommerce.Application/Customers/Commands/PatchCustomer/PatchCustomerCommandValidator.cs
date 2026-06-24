using FluentValidation;

namespace Ecommerce.Application.Customers.Commands.PatchCustomer;

public sealed class PatchCustomerCommandValidator : AbstractValidator<PatchCustomerCommand>
{
    public PatchCustomerCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();

        RuleFor(command => command)
            .Must(command => command.Name is not null || command.Email is not null)
            .WithMessage("Informe ao menos um campo para alteração.")
            .OverridePropertyName("Fields");

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(150)
            .When(command => command.Name is not null);

        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress()
            .When(command => command.Email is not null);
    }
}
