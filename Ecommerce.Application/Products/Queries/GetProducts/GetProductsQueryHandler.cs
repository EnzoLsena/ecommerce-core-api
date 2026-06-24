using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Products.Abstractions;
using Ecommerce.Application.Products.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler(
    IProductReadStore readStore,
    ILogger<GetProductsQueryHandler> logger)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductReadModel>>
{
    public async Task<PagedResult<ProductReadModel>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await readStore.GetPagedAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        logger.LogInformation(
            "Página de produtos consultada com página {Page}, tamanho {PageSize} e total de itens {TotalItems}",
            result.Page,
            result.PageSize,
            result.TotalItems);

        return result;
    }
}
