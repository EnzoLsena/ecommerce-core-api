using FluentValidation;

namespace Ecommerce.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId).NotEmpty();
        RuleFor(command => command.Items)
            .NotNull()
            .NotEmpty()
            .Must(items => items is not null &&
                items.Select(item => item.ProductId).Distinct().Count() == items.Count)
            .WithMessage("Products cannot be duplicated in an order.");

        RuleForEach(command => command.Items).ChildRules(item =>
        {
            item.RuleFor(value => value.ProductId).NotEmpty();
            item.RuleFor(value => value.Quantity).GreaterThan(0);
        });
    }
}
