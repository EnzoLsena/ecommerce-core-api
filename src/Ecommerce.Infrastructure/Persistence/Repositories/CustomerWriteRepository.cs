using Ecommerce.Application.Customers.Abstractions;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class CustomerWriteRepository(EcommerceDbContext ecommerceDbContext)
    : ICustomerWriteRepository
{
    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        ecommerceDbContext.Customers.SingleOrDefaultAsync(
            customer => customer.Id == id,
            cancellationToken);

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        ecommerceDbContext.Customers.SingleOrDefaultAsync(
            customer => customer.Email == email.Trim(),
            cancellationToken);

    public Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        ecommerceDbContext.Customers.Add(customer);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken)
    {
        ecommerceDbContext.Customers.Update(customer);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Customer customer, CancellationToken cancellationToken)
    {
        customer.Delete();
        return Task.CompletedTask;
    }
}
