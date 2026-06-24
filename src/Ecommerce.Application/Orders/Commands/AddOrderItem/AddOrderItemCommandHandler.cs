using AutoMapper;
using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Orders.Commands.AddOrderItem;

public sealed class AddOrderItemCommandHandler(
    IOrderWriteRepository writeRepository,
    IUnitOfWork unitOfWork,
    IOrderReadStore readStore,
    IMapper mapper,
    ILogger<AddOrderItemCommandHandler> logger) : IRequestHandler<AddOrderItemCommand, bool>
{
    public async Task<bool> Handle(
        AddOrderItemCommand request,
        CancellationToken cancellationToken)
    {
        var order = await writeRepository.GetByIdAsync(request.OrderId, cancellationToken);

        var unitPrice = await writeRepository.GetProductPriceAsync(
            request.ProductId,
            cancellationToken);

        if (order is null || !unitPrice.HasValue)
        {
            logger.LogWarning(
                "Pedido {OrderId} ou produto {ProductId} não encontrado para inclusão do item",
                request.OrderId,
                request.ProductId);

            return false;
        }

        order.AddItem(request.ProductId, request.Quantity, unitPrice.Value);
        await writeRepository.UpdateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedOrder = await writeRepository.GetByIdAsync(order.Id, cancellationToken)
            ?? throw new InvalidOperationException("O pedido atualizado não foi encontrado.");

        await readStore.TryUpsertAsync(
            mapper.Map<OrderReadModel>(updatedOrder),
            cancellationToken);

        logger.LogInformation(
            "Produto {ProductId} adicionado ao pedido {OrderId} com quantidade {Quantity}",
            request.ProductId,
            order.Id,
            request.Quantity);

        return true;
    }
}
