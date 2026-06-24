using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Customers.Models;
using MediatR;

namespace Ecommerce.Application.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery(int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<CustomerReadModel>>;
