using AutoMapper;
using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Orders.Commands.ShipOrder;

public sealed class ShipOrderCommandHandler(
    IOrderWriteRepository writeRepository,
    IUnitOfWork unitOfWork,
    IOrderReadStore readStore,
    IMapper mapper,
    ILogger<ShipOrderCommandHandler> logger) : IRequestHandler<ShipOrderCommand, bool>
{
    public async Task<bool> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await writeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("Pedido {OrderId} não encontrado para envio", request.Id);
            return false;
        }

        order.MarkAsShipped();
        await writeRepository.UpdateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await readStore.TryUpsertAsync(mapper.Map<OrderReadModel>(order), cancellationToken);

        logger.LogInformation("Pedido {OrderId} enviado", order.Id);

        return true;
    }
}
