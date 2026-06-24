using AutoMapper;
using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Customers.Abstractions;
using Ecommerce.Application.Customers.Models;
using Ecommerce.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Customers.Commands.PatchCustomer;

public sealed class PatchCustomerCommandHandler(
    ICustomerWriteRepository writeRepository,
    IUnitOfWork unitOfWork,
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

        if (request.Email is not null)
        {
            var customerWithEmail = await writeRepository.GetByEmailAsync(
                request.Email,
                cancellationToken);

            if (customerWithEmail is not null && customerWithEmail.Id != request.Id)
                throw new DomainException("Já existe um cliente cadastrado com este e-mail.");
        }

        customer.ChangeDetails(
            request.Name ?? customer.Name,
            request.Email ?? customer.Email);

        await writeRepository.UpdateAsync(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
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
