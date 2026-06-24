using Ecommerce.Application.Common.Abstractions;

namespace Ecommerce.Infrastructure.Persistence;

public sealed class UnitOfWork(EcommerceDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        dbContext.ChangeTracker.Clear();
    }
}
