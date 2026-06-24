using AutoMapper;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Orders.Commands.ProcessOrder;

public sealed class ProcessOrderCommandHandler(
    IOrderWriteRepository writeRepository,
    IOrderReadStore readStore,
    IMapper mapper,
    ILogger<ProcessOrderCommandHandler> logger) : IRequestHandler<ProcessOrderCommand, bool>
{
    public async Task<bool> Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await writeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("Pedido {OrderId} não encontrado para processamento", request.Id);
            return false;
        }

        order.MarkAsProcessed();
        await writeRepository.UpdateAsync(order, cancellationToken);
        await readStore.TryUpsertAsync(mapper.Map<OrderReadModel>(order), cancellationToken);

        logger.LogInformation("Pedido {OrderId} processado", order.Id);

        return true;
    }
}
