using Ecommerce.Application.Customers.Abstractions;
using Ecommerce.Application.Customers.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Customers.Queries.GetCustomerById;

public sealed class GetCustomerByIdQueryHandler(
    ICustomerReadStore readStore,
    ILogger<GetCustomerByIdQueryHandler> logger)
    : IRequestHandler<GetCustomerByIdQuery, CustomerReadModel?>
{
    public async Task<CustomerReadModel?> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await readStore.GetByIdAsync(request.Id, cancellationToken);

        logger.LogInformation(
            "Consulta do cliente {CustomerId} concluída com resultado encontrado {CustomerFound}",
            request.Id,
            customer is not null);

        return customer;
    }
}
