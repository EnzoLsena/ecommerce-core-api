using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class OrderWriteRepository(EcommerceDbContext ecommerceDbContext)
    : IOrderWriteRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        ecommerceDbContext.Orders
            .IgnoreQueryFilters()
            .Where(order => order.DeletedAt == null)
            .Include(order => order.Customer)
            .Include(order => order.Items.Where(item => item.DeletedAt == null))
            .ThenInclude(item => item.Product)
            .AsSplitQuery()
            .SingleOrDefaultAsync(order => order.Id == id, cancellationToken);

    public Task<bool> CustomerExistsAsync(Guid id, CancellationToken cancellationToken) =>
        ecommerceDbContext.Customers.AnyAsync(customer => customer.Id == id, cancellationToken);

    public Task<decimal?> GetProductPriceAsync(Guid id, CancellationToken cancellationToken) =>
        ecommerceDbContext.Products
            .Where(product => product.Id == id)
            .Select(product => (decimal?)product.Price)
            .SingleOrDefaultAsync(cancellationToken);

    public Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        ecommerceDbContext.Orders.Add(order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        ecommerceDbContext.Entry(order).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Order order, CancellationToken cancellationToken)
    {
        order.Delete();
        return Task.CompletedTask;
    }
}
