using FluentValidation;

namespace Ecommerce.Application.Orders.Queries.GetOrdersByCustomer;

public sealed class GetOrdersByCustomerQueryValidator
    : AbstractValidator<GetOrdersByCustomerQuery>
{
    public GetOrdersByCustomerQueryValidator()
    {
        RuleFor(query => query.CustomerId).NotEmpty();
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
    }
}
