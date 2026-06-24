using Ecommerce.Application.Products.Abstractions;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class ProductWriteRepository(EcommerceDbContext ecommerceDbContext)
    : IProductWriteRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        ecommerceDbContext.Products.SingleOrDefaultAsync(
            product => product.Id == id,
            cancellationToken);

    public Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        ecommerceDbContext.Products.Add(product);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        ecommerceDbContext.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product, CancellationToken cancellationToken)
    {
        product.Delete();
        return Task.CompletedTask;
    }
}
