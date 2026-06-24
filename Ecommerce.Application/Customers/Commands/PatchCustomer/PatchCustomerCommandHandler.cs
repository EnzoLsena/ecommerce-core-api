using AutoMapper;
using Ecommerce.Application.Customers.Abstractions;
using Ecommerce.Application.Customers.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Customers.Commands.PatchCustomer;

public sealed class PatchCustomerCommandHandler(
    ICustomerWriteRepository writeRepository,
    ICustomerReadStore readStore,
    IMapper mapper,
    ILogger<PatchCustomerCommandHandler> logger) : IRequestHandler<PatchCustomerCommand, bool>
{
    public async Task<bool> Handle(
        PatchCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await writeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (customer is null)
        {
            logger.LogWarning("Cliente {CustomerId} não encontrado para atualização parcial", request.Id);
            return false;
        }

        customer.ChangeDetails(
            request.Name ?? customer.Name,
            request.Email ?? customer.Email);

        await writeRepository.UpdateAsync(customer, cancellationToken);
        await readStore.TryUpsertAsync(
            mapper.Map<CustomerReadModel>(customer),
            cancellationToken);

        logger.LogInformation(
            "Cliente {CustomerId} atualizado parcialmente com alteração de nome {NameChanged} e alteração de e-mail {EmailChanged}",
            customer.Id,
            request.Name is not null,
            request.Email is not null);

        return true;
    }
}
