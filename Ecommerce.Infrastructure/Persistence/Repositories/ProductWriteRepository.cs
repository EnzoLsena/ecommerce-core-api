using Ecommerce.Application.Products.Abstractions;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class ProductWriteRepository(EcommerceDbContext dbContext)
    : IProductWriteRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Products.SingleOrDefaultAsync(
            product => product.Id == id,
            cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken)
    {
        product.Delete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
