namespace Ecommerce.Application.Common.Abstractions;

public interface ICache
{
    Task<bool> Delete<T>(
        string id,
        CancellationToken cancellationToken = default,
        params string[] parameters);

    Task<T?> Get<T>(
        string id,
        CancellationToken cancellationToken = default,
        params string[] parameters);

    Task Store<T>(
        string id,
        T value,
        TimeSpan? ttl = null,
        CancellationToken cancellationToken = default,
        params string[] parameters);
}
