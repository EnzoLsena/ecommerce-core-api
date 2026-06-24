using AutoMapper;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler(
    IOrderWriteRepository writeRepository,
    IOrderReadStore readStore,
    IMapper mapper,
    ILogger<CancelOrderCommandHandler> logger) : IRequestHandler<CancelOrderCommand, bool>
{
    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await writeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("Pedido {OrderId} não encontrado para cancelamento", request.Id);
            return false;
        }

        order.Cancel();
        await writeRepository.UpdateAsync(order, cancellationToken);
        await readStore.TryUpsertAsync(mapper.Map<OrderReadModel>(order), cancellationToken);

        logger.LogInformation("Pedido {OrderId} cancelado", order.Id);

        return true;
    }
}
