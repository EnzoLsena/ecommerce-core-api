using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Orders.Models;
using MediatR;

namespace Ecommerce.Application.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery(int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<OrderReadModel>>;
