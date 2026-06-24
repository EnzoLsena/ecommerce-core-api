using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Customers.Models;

namespace Ecommerce.Application.Customers.Abstractions;

public interface ICustomerReadStore
{
    Task TryUpsertAsync(CustomerReadModel customer, CancellationToken cancellationToken);
    Task TryDeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<CustomerReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<CustomerReadModel>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
