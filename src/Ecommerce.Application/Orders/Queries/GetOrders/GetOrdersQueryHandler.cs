using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler(
    IOrderReadStore readStore,
    ILogger<GetOrdersQueryHandler> logger)
    : IRequestHandler<GetOrdersQuery, PagedResult<OrderReadModel>>
{
    public async Task<PagedResult<OrderReadModel>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var result = await readStore.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Code,
            request.MinTotalAmount,
            request.MaxTotalAmount,
            request.MinTotalItems,
            request.MaxTotalItems,
            request.PaidFrom,
            request.PaidTo,
            request.CanceledFrom,
            request.CanceledTo,
            cancellationToken);

        logger.LogInformation(
            "Página de pedidos consultada com página {Page}, tamanho {PageSize} e total de itens {TotalItems}",
            result.Page,
            result.PageSize,
            result.TotalItems);

        return result;
    }
}
