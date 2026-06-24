using FluentValidation;

namespace Ecommerce.Application.Products.Commands.PatchProduct;

public sealed class PatchProductCommandValidator : AbstractValidator<PatchProductCommand>
{
    public PatchProductCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();

        RuleFor(command => command)
            .Must(command => command.Name is not null || command.Price.HasValue)
            .WithMessage("Informe ao menos um campo para alteração.")
            .OverridePropertyName("Fields");

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200)
            .When(command => command.Name is not null);

        RuleFor(command => command.Price)
            .GreaterThanOrEqualTo(0)
            .When(command => command.Price.HasValue);
    }
}
