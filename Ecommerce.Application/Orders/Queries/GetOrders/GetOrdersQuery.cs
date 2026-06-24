using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Orders.Models;
using MediatR;

namespace Ecommerce.Application.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Code = null,
    decimal? MinTotalAmount = null,
    decimal? MaxTotalAmount = null,
    int? MinTotalItems = null,
    int? MaxTotalItems = null,
    DateTime? PaidFrom = null,
    DateTime? PaidTo = null,
    DateTime? CanceledFrom = null,
    DateTime? CanceledTo = null)
    : IRequest<PagedResult<OrderReadModel>>;
