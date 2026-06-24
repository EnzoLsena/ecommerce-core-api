using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class OrderWriteRepository(EcommerceDbContext dbContext)
    : IOrderWriteRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Orders
            .IgnoreQueryFilters()
            .Where(order => order.DeletedAt == null)
            .Include(order => order.Customer)
            .Include(order => order.Items.Where(item => item.DeletedAt == null))
            .ThenInclude(item => item.Product)
            .AsSplitQuery()
            .SingleOrDefaultAsync(order => order.Id == id, cancellationToken);

    public Task<bool> CustomerExistsAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Customers.AnyAsync(customer => customer.Id == id, cancellationToken);

    public Task<bool> ProductExistsAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Products.AnyAsync(product => product.Id == id, cancellationToken);

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);
        dbContext.ChangeTracker.Clear();
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        dbContext.Entry(order).State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);
        dbContext.ChangeTracker.Clear();
    }

    public async Task DeleteAsync(Order order, CancellationToken cancellationToken)
    {
        order.Delete();
        await dbContext.SaveChangesAsync(cancellationToken);
        dbContext.ChangeTracker.Clear();
    }
}
