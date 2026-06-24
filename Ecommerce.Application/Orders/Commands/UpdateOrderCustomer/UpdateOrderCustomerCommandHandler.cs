using AutoMapper;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Orders.Commands.UpdateOrderCustomer;

public sealed class UpdateOrderCustomerCommandHandler(
    IOrderWriteRepository writeRepository,
    IOrderReadStore readStore,
    IMapper mapper,
    ILogger<UpdateOrderCustomerCommandHandler> logger) : IRequestHandler<UpdateOrderCustomerCommand, bool>
{
    public async Task<bool> Handle(
        UpdateOrderCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var order = await writeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null ||
            !await writeRepository.CustomerExistsAsync(request.CustomerId, cancellationToken))
        {
            logger.LogWarning(
                "Pedido {OrderId} ou cliente {CustomerId} não encontrado para atualização",
                request.Id,
                request.CustomerId);

            return false;
        }

        order.UpdateCustomer(request.CustomerId);
        await writeRepository.UpdateAsync(order, cancellationToken);

        var updatedOrder = await writeRepository.GetByIdAsync(order.Id, cancellationToken)
            ?? throw new InvalidOperationException("O pedido atualizado não foi encontrado.");

        await readStore.TryUpsertAsync(
            mapper.Map<OrderReadModel>(updatedOrder),
            cancellationToken);

        logger.LogInformation("Cliente do pedido {OrderId} atualizado para {CustomerId}", order.Id, order.CustomerId);

        return true;
    }
}
