using AutoMapper;
using Ecommerce.Application.Products.Abstractions;
using Ecommerce.Application.Products.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    IProductWriteRepository writeRepository,
    IProductReadStore readStore,
    IMapper mapper,
    ILogger<UpdateProductCommandHandler> logger) : IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await writeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            logger.LogWarning(
                "Produto {ProductId} não encontrado para atualização",
                request.Id);

            return false;
        }

        product.ChangeDetails(request.Name, request.Price);
        await writeRepository.UpdateAsync(product, cancellationToken);

        await readStore.TryUpsertAsync(
            mapper.Map<ProductReadModel>(product),
            cancellationToken);

        logger.LogInformation(
            "Produto {ProductId} atualizado",
            product.Id);

        return true;
    }
}
