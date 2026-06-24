using AutoMapper;
using Ecommerce.Application.Customers.Abstractions;
using Ecommerce.Application.Customers.Models;
using Ecommerce.Domain.Entities;
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
        var customer = new Customer(request.Name, request.Email);

        await writeRepository.AddAsync(customer, cancellationToken);
        await readStore.TryUpsertAsync(
            mapper.Map<CustomerReadModel>(customer),
            cancellationToken);

        logger.LogInformation("Cliente {CustomerId} criado", customer.Id);

        return customer.Id;
    }
}
