using AutoMapper;
using Ecommerce.Application.Customers.Abstractions;
using Ecommerce.Application.Customers.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerCommandHandler(
    ICustomerWriteRepository writeRepository,
    ICustomerReadStore readStore,
    IMapper mapper,
    ILogger<UpdateCustomerCommandHandler> logger) : IRequestHandler<UpdateCustomerCommand, bool>
{
    public async Task<bool> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await writeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (customer is null)
        {
            logger.LogWarning("Cliente {CustomerId} não encontrado para atualização", request.Id);
            return false;
        }

        customer.ChangeDetails(request.Name, request.Email);
        await writeRepository.UpdateAsync(customer, cancellationToken);
        await readStore.TryUpsertAsync(
            mapper.Map<CustomerReadModel>(customer),
            cancellationToken);

        logger.LogInformation("Cliente {CustomerId} atualizado", customer.Id);

        return true;
    }
}
