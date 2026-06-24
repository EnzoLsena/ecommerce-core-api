using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Products.Models;

namespace Ecommerce.Application.Products.Abstractions;

public interface IProductReadStore
{
    Task TryUpsertAsync(ProductReadModel product, CancellationToken cancellationToken);
    Task TryDeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<ProductReadModel>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
