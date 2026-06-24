using Ecommerce.Application.Products.Abstractions;
using Ecommerce.Application.Products.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(
    IProductReadStore readStore,
    ILogger<GetProductByIdQueryHandler> logger)
    : IRequestHandler<GetProductByIdQuery, ProductReadModel?>
{
    public async Task<ProductReadModel?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await readStore.GetByIdAsync(request.Id, cancellationToken);

        logger.LogInformation(
            "Consulta do produto {ProductId} concluída com resultado encontrado {ProductFound}",
            request.Id,
            product is not null);

        return product;
    }
}
