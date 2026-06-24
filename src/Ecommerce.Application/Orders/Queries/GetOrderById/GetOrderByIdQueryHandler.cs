using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(
    IOrderReadStore readStore,
    ILogger<GetOrderByIdQueryHandler> logger)
    : IRequestHandler<GetOrderByIdQuery, OrderReadModel?>
{
    public async Task<OrderReadModel?> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await readStore.GetByIdAsync(request.Id, cancellationToken);

        logger.LogInformation(
            "Consulta do pedido {OrderId} concluída com resultado encontrado {OrderFound}",
            request.Id,
            order is not null);

        return order;
    }
}
