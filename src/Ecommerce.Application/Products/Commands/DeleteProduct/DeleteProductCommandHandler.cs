using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Products.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler(
    IProductWriteRepository writeRepository,
    IUnitOfWork unitOfWork,
    IProductReadStore readStore,
    ILogger<DeleteProductCommandHandler> logger) : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await writeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            logger.LogWarning(
                "Produto {ProductId} não encontrado para exclusão",
                request.Id);

            return false;
        }

        await writeRepository.DeleteAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await readStore.TryDeleteAsync(product.Id, cancellationToken);

        logger.LogInformation(
            "Produto {ProductId} excluído",
            product.Id);

        return true;
    }
}
