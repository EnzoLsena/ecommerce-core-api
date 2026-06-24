using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Customers.Abstractions;

public interface ICustomerWriteRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken);
    Task DeleteAsync(Customer customer, CancellationToken cancellationToken);
}
