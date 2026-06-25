using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Orders.Models;
using MediatR;

namespace Ecommerce.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQuery : IRequest<PagedResult<OrderReadModel>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Code { get; init; }
    public decimal? MinTotalAmount { get; init; }
    public decimal? MaxTotalAmount { get; init; }
    public int? MinTotalItems { get; init; }
    public int? MaxTotalItems { get; init; }
    public DateTime? PaidFrom { get; init; }
    public DateTime? PaidTo { get; init; }
    public DateTime? CanceledFrom { get; init; }
    public DateTime? CanceledTo { get; init; }
}
