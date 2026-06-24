using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Orders.Models;

namespace Ecommerce.Application.Orders.Abstractions;

public interface IOrderReadStore
{
    Task TryUpsertAsync(OrderReadModel order, CancellationToken cancellationToken);
    Task TryDeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<OrderReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<OrderReadModel>> GetByCustomerAsync(
        Guid customerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
    Task<PagedResult<OrderReadModel>> GetPagedAsync(
        int page,
        int pageSize,
        string? code,
        decimal? minTotalAmount,
        decimal? maxTotalAmount,
        int? minTotalItems,
        int? maxTotalItems,
        DateTime? paidFrom,
        DateTime? paidTo,
        DateTime? canceledFrom,
        DateTime? canceledTo,
        CancellationToken cancellationToken);
}
