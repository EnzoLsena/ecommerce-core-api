using AutoMapper;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Orders.Commands.ChangeOrderItem;

public sealed class ChangeOrderItemCommandHandler(
    IOrderWriteRepository writeRepository,
    IOrderReadStore readStore,
    IMapper mapper,
    ILogger<ChangeOrderItemCommandHandler> logger) : IRequestHandler<ChangeOrderItemCommand, bool>
{
    public async Task<bool> Handle(
        ChangeOrderItemCommand request,
        CancellationToken cancellationToken)
    {
        var order = await writeRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("Pedido {OrderId} não encontrado para alteração do item", request.OrderId);
            return false;
        }

        order.ChangeItem(request.ProductId, request.Quantity, request.UnitPrice);
        await writeRepository.UpdateAsync(order, cancellationToken);
        await readStore.TryUpsertAsync(
            mapper.Map<OrderReadModel>(order),
            cancellationToken);

        logger.LogInformation(
            "Produto {ProductId} alterado no pedido {OrderId} para quantidade {Quantity}",
            request.ProductId,
            order.Id,
            request.Quantity);

        return true;
    }
}
