using AutoMapper;
using Ecommerce.Application.Products.Abstractions;
using Ecommerce.Application.Products.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Products.Commands.PatchProduct;

public sealed class PatchProductCommandHandler(
    IProductWriteRepository writeRepository,
    IProductReadStore readStore,
    IMapper mapper,
    ILogger<PatchProductCommandHandler> logger) : IRequestHandler<PatchProductCommand, bool>
{
    public async Task<bool> Handle(
        PatchProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await writeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            logger.LogWarning(
                "Produto {ProductId} não encontrado para atualização parcial",
                request.Id);

            return false;
        }

        product.ChangeDetails(
            request.Name ?? product.Name,
            request.Price ?? product.Price);

        await writeRepository.UpdateAsync(product, cancellationToken);

        await readStore.TryUpsertAsync(
            mapper.Map<ProductReadModel>(product),
            cancellationToken);

        logger.LogInformation(
            "Produto {ProductId} atualizado parcialmente com alteração de nome {NameChanged} e alteração de preço {PriceChanged}",
            product.Id,
            request.Name is not null,
            request.Price.HasValue);

        return true;
    }
}
