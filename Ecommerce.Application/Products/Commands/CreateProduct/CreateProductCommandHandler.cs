using AutoMapper;
using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Products.Abstractions;
using Ecommerce.Application.Products.Models;
using Ecommerce.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductWriteRepository writeRepository,
    IUnitOfWork unitOfWork,
    IProductReadStore readStore,
    IMapper mapper,
    ILogger<CreateProductCommandHandler> logger) : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = new Product(request.Name, request.Price);

        await writeRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await readStore.TryUpsertAsync(
            mapper.Map<ProductReadModel>(product),
            cancellationToken);

        logger.LogInformation(
            "Produto {ProductId} criado",
            product.Id);

        return product.Id;
    }
}
