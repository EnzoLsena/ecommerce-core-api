using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ecommerce.Application.Common.Abstractions;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;

namespace Ecommerce.Infrastructure.Persistence.Mongo;

public sealed class CachedOrderReadStore(
    MongoOrderReadStore inner,
    ICache cache) : IOrderReadStore
{
    private const string VersionId = "orders";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan DetailCacheTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ListCacheTtl = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan VersionCacheTtl = TimeSpan.FromDays(30);

    public async Task TryUpsertAsync(
        OrderReadModel order,
        CancellationToken cancellationToken)
    {
        await inner.TryUpsertAsync(order, cancellationToken);
        await cache.Store(order.Id.ToString("N"), order, DetailCacheTtl, cancellationToken);
        await RefreshVersionAsync(cancellationToken);
    }

    public async Task TryDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await inner.TryDeleteAsync(id, cancellationToken);
        await cache.Delete<OrderReadModel>(id.ToString("N"), cancellationToken);
        await RefreshVersionAsync(cancellationToken);
    }

    public async Task<OrderReadModel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cachedOrder = await cache.Get<OrderReadModel>(
            id.ToString("N"),
            cancellationToken);

        if (cachedOrder is not null)
            return cachedOrder;

        var order = await inner.GetByIdAsync(id, cancellationToken);

        if (order is not null)
            await cache.Store(
                id.ToString("N"),
                order,
                DetailCacheTtl,
                cancellationToken);

        return order;
    }

    public async Task<PagedResult<OrderReadModel>> GetByCustomerAsync(
        Guid customerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var version = await GetVersionAsync(cancellationToken);
        var id = $"customer:{version}:{customerId:N}:{page}:{pageSize}";
        var cachedResult = await cache.Get<PagedResult<OrderReadModel>>(
            id,
            cancellationToken);

        if (cachedResult is not null)
            return cachedResult;

        var result = await inner.GetByCustomerAsync(
            customerId,
            page,
            pageSize,
            cancellationToken);

        await cache.Store(id, result, ListCacheTtl, cancellationToken);

        return result;
    }

    public async Task<PagedResult<OrderReadModel>> GetPagedAsync(
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
        CancellationToken cancellationToken)
    {
        var version = await GetVersionAsync(cancellationToken);
        var id = $"paged:{version}:{Fingerprint(
            page,
            pageSize,
            code,
            minTotalAmount,
            maxTotalAmount,
            minTotalItems,
            maxTotalItems,
            paidFrom,
            paidTo,
            canceledFrom,
            canceledTo)}";
        var cachedResult = await cache.Get<PagedResult<OrderReadModel>>(
            id,
            cancellationToken);

        if (cachedResult is not null)
            return cachedResult;

        var result = await inner.GetPagedAsync(
            page,
            pageSize,
            code,
            minTotalAmount,
            maxTotalAmount,
            minTotalItems,
            maxTotalItems,
            paidFrom,
            paidTo,
            canceledFrom,
            canceledTo,
            cancellationToken);

        await cache.Store(id, result, ListCacheTtl, cancellationToken);

        return result;
    }

    private static string Fingerprint(params object?[] values)
    {
        var normalized = JsonSerializer.Serialize(values, JsonOptions);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));

        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private async Task<string> GetVersionAsync(CancellationToken cancellationToken)
    {
        var version = await cache.Get<OrderCacheVersion>(
            VersionId,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(version?.Value))
            return version.Value;

        var newVersion = Guid.NewGuid().ToString("N");
        await cache.Store(
            VersionId,
            new OrderCacheVersion(newVersion),
            VersionCacheTtl,
            cancellationToken);

        return newVersion;
    }

    private async Task RefreshVersionAsync(CancellationToken cancellationToken)
    {
        await cache.Store(
            VersionId,
            new OrderCacheVersion(Guid.NewGuid().ToString("N")),
            VersionCacheTtl,
            cancellationToken);
    }

    private sealed record OrderCacheVersion(string Value);
}
