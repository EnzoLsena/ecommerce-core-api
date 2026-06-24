using AutoMapper;
using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;
using Ecommerce.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IOrderWriteRepository writeRepository,
    IUnitOfWork unitOfWork,
    IOrderReadStore readStore,
    IMapper mapper,
    ILogger<CreateOrderCommandHandler> logger) : IRequestHandler<CreateOrderCommand, Guid?>
{
    public async Task<Guid?> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (!await writeRepository.CustomerExistsAsync(request.CustomerId, cancellationToken))
        {
            logger.LogWarning("Cliente {CustomerId} não encontrado para criação do pedido", request.CustomerId);
            return null;
        }

        var order = new Order(request.CustomerId);
        await writeRepository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var savedOrder = await writeRepository.GetByIdAsync(order.Id, cancellationToken)
            ?? throw new InvalidOperationException("O pedido salvo não foi encontrado.");

        await readStore.TryUpsertAsync(
            mapper.Map<OrderReadModel>(savedOrder),
            cancellationToken);

        logger.LogInformation("Pedido {OrderId} criado para o cliente {CustomerId}", order.Id, order.CustomerId);

        return order.Id;
    }
}
