using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Orders.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Orders.Commands.DeleteOrder;

public sealed class DeleteOrderCommandHandler(
    IOrderWriteRepository writeRepository,
    IUnitOfWork unitOfWork,
    IOrderReadStore readStore,
    ILogger<DeleteOrderCommandHandler> logger) : IRequestHandler<DeleteOrderCommand, bool>
{
    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await writeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("Pedido {OrderId} não encontrado para exclusão", request.Id);
            return false;
        }

        await writeRepository.DeleteAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await readStore.TryDeleteAsync(order.Id, cancellationToken);

        logger.LogInformation("Pedido {OrderId} excluído", order.Id);

        return true;
    }
}
