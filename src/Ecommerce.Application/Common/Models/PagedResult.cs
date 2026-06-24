namespace Ecommerce.Application.Common.Models;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    long TotalItems,
    int Page,
    int PageSize)
{
    public int TotalPages => TotalItems == 0
        ? 0
        : (int)Math.Ceiling((double)TotalItems / PageSize);
}
