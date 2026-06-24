using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Customers.Abstractions;
using Ecommerce.Application.Customers.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Customers.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler(
    ICustomerReadStore readStore,
    ILogger<GetCustomersQueryHandler> logger)
    : IRequestHandler<GetCustomersQuery, PagedResult<CustomerReadModel>>
{
    public async Task<PagedResult<CustomerReadModel>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var result = await readStore.GetPagedAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        logger.LogInformation(
            "Página de clientes consultada com página {Page}, tamanho {PageSize} e total de itens {TotalItems}",
            result.Page,
            result.PageSize,
            result.TotalItems);

        return result;
    }
}
