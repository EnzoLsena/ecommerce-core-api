using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Orders.Abstractions;

public interface IOrderWriteRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> CustomerExistsAsync(Guid id, CancellationToken cancellationToken);
    Task<decimal?> GetProductPriceAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task UpdateAsync(Order order, CancellationToken cancellationToken);
    Task DeleteAsync(Order order, CancellationToken cancellationToken);
}
