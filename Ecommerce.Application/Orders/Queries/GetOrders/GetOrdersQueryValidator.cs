using FluentValidation;

namespace Ecommerce.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Code).MaximumLength(32);
        RuleFor(query => query.MinTotalAmount).GreaterThanOrEqualTo(0);
        RuleFor(query => query.MaxTotalAmount).GreaterThanOrEqualTo(0);
        RuleFor(query => query.MinTotalItems).GreaterThanOrEqualTo(0);
        RuleFor(query => query.MaxTotalItems).GreaterThanOrEqualTo(0);

        RuleFor(query => query.MaxTotalAmount)
            .GreaterThanOrEqualTo(query => query.MinTotalAmount!.Value)
            .When(query => query.MinTotalAmount.HasValue && query.MaxTotalAmount.HasValue);

        RuleFor(query => query.MaxTotalItems)
            .GreaterThanOrEqualTo(query => query.MinTotalItems!.Value)
            .When(query => query.MinTotalItems.HasValue && query.MaxTotalItems.HasValue);

        RuleFor(query => query.PaidTo)
            .GreaterThanOrEqualTo(query => query.PaidFrom!.Value)
            .When(query => query.PaidFrom.HasValue && query.PaidTo.HasValue);

        RuleFor(query => query.CanceledTo)
            .GreaterThanOrEqualTo(query => query.CanceledFrom!.Value)
            .When(query => query.CanceledFrom.HasValue && query.CanceledTo.HasValue);
    }
}
