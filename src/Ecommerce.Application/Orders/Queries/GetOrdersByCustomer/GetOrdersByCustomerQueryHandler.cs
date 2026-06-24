using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Customers.Abstractions;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Orders.Queries.GetOrdersByCustomer;

public sealed class GetOrdersByCustomerQueryHandler(
    ICustomerReadStore customerReadStore,
    IOrderReadStore orderReadStore,
    ILogger<GetOrdersByCustomerQueryHandler> logger)
    : IRequestHandler<GetOrdersByCustomerQuery, PagedResult<OrderReadModel>?>
{
    public async Task<PagedResult<OrderReadModel>?> Handle(
        GetOrdersByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await customerReadStore.GetByIdAsync(
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            logger.LogWarning(
                "Cliente {CustomerId} não encontrado para consulta de pedidos",
                request.CustomerId);
            return null;
        }

        return await orderReadStore.GetByCustomerAsync(
            request.CustomerId,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}
