using AutoMapper;
using Ecommerce.Application.Customers.Abstractions;
using Ecommerce.Application.Customers.Models;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler(
    ICustomerWriteRepository writeRepository,
    ICustomerReadStore readStore,
    IMapper mapper,
    ILogger<CreateCustomerCommandHandler> logger) : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        if (await writeRepository.GetByEmailAsync(request.Email, cancellationToken) is not null)
            throw new DomainException("Já existe um cliente cadastrado com este e-mail.");

        var customer = new Customer(request.Name, request.Email);

        await writeRepository.AddAsync(customer, cancellationToken);
        await readStore.TryUpsertAsync(
            mapper.Map<CustomerReadModel>(customer),
            cancellationToken);

        logger.LogInformation("Cliente {CustomerId} criado", customer.Id);

        return customer.Id;
    }
}
