using Ecommerce.Application.Customers.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Customers.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandHandler(
    ICustomerWriteRepository writeRepository,
    ICustomerReadStore readStore,
    ILogger<DeleteCustomerCommandHandler> logger) : IRequestHandler<DeleteCustomerCommand, bool>
{
    public async Task<bool> Handle(
        DeleteCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await writeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (customer is null)
        {
            logger.LogWarning("Cliente {CustomerId} não encontrado para exclusão", request.Id);
            return false;
        }

        await writeRepository.DeleteAsync(customer, cancellationToken);
        await readStore.TryDeleteAsync(customer.Id, cancellationToken);

        logger.LogInformation("Cliente {CustomerId} excluído", customer.Id);

        return true;
    }
}
