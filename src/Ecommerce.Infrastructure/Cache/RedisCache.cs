using System.Text;
using System.Text.Json;
using Ecommerce.Application.Common.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Ecommerce.Infrastructure.Cache;

public sealed class RedisCache : ICache, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly ILogger<RedisCache> _logger;
    private readonly TimeSpan _defaultTtl = TimeSpan.FromMinutes(15);

    public RedisCache(IConfiguration configuration, ILogger<RedisCache> logger)
    {
        var connectionString = Environment.GetEnvironmentVariable("REDIS_URL")
            ?? configuration["Redis:ConnectionString"]
            ?? configuration.GetConnectionString("RedisConnection")
            ?? throw new InvalidOperationException("Redis connection string was not configured.");

        _redis = ConnectionMultiplexer.Connect(connectionString);
        _database = _redis.GetDatabase();
        _logger = logger;
    }

    public async Task<bool> Delete<T>(
        string id,
        CancellationToken cancellationToken = default,
        params string[] parameters)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _database.KeyDeleteAsync(GetKey<T>(id, parameters));
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Erro ao deletar cache Redis para {CacheType}:{CacheId}.", typeof(T).Name, id);
            return false;
        }
    }

    public async Task<T?> Get<T>(
        string id,
        CancellationToken cancellationToken = default,
        params string[] parameters)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var value = await _database.StringGetAsync(GetKey<T>(id, parameters));

            return value.HasValue
                ? JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions)
                : default;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Erro ao ler cache Redis para {CacheType}:{CacheId}.", typeof(T).Name, id);
            return default;
        }
    }

    public async Task Store<T>(
        string id,
        T value,
        TimeSpan? ttl = null,
        CancellationToken cancellationToken = default,
        params string[] parameters)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var cache = JsonSerializer.Serialize(value, JsonOptions);

            await _database.StringSetAsync(
                GetKey<T>(id, parameters),
                cache,
                ttl ?? _defaultTtl);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Erro ao gravar cache Redis para {CacheType}:{CacheId}.", typeof(T).Name, id);
        }
    }

    public void Dispose()
    {
        _redis.Dispose();
    }

    private static string GetKey<T>(string id, string[] parameters)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Chave não pode ser nula ou vazia.", nameof(id));

        var key = $"{typeof(T).Name.ToLowerInvariant()}:{id}";

        return parameters.Length == 0
            ? key
            : GenerateKeyWithParameters(key, parameters);
    }

    private static string GenerateKeyWithParameters(string key, string[] parameters)
    {
        var builder = new StringBuilder(key);

        foreach (var parameter in parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameter))
                builder.Append('&').Append(parameter);
        }

        return builder.ToString();
    }
}
