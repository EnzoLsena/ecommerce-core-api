using Ecommerce.Application.Customers.Abstractions;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class CustomerWriteRepository(EcommerceDbContext dbContext)
    : ICustomerWriteRepository
{
    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Customers.SingleOrDefaultAsync(
            customer => customer.Id == id,
            cancellationToken);

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken)
    {
        dbContext.Customers.Update(customer);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken cancellationToken)
    {
        customer.Delete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
