using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Products.Abstractions;
using Ecommerce.Application.Products.Models;
using Ecommerce.Infrastructure.Persistence.Mongo.Documents;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Ecommerce.Infrastructure.Persistence.Mongo;

public sealed class MongoProductReadStore(
    IMongoDatabase database,
    ILogger<MongoProductReadStore> logger) : IProductReadStore
{
    private readonly IMongoCollection<ProductDocument> _products =
        database.GetCollection<ProductDocument>("products");

    public async Task TryUpsertAsync(
        ProductReadModel product,
        CancellationToken cancellationToken)
    {
        var document = new ProductDocument
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };

        try
        {
            await _products.ReplaceOneAsync(
                item => item.Id == product.Id,
                document,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken);

        }
        catch (MongoException exception)
        {
            logger.LogWarning(
                exception,
                "Produto {ProductId} salvo no SQL Server, mas sua projeção no MongoDB falhou.",
                product.Id);
        }
    }

    public async Task<ProductReadModel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var document = await _products
            .Find(product => product.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document is null ? null : Map(document);
    }

    public async Task TryDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _products.DeleteOneAsync(
                product => product.Id == id,
                cancellationToken);
        }
        catch (MongoException exception)
        {
            logger.LogWarning(
                exception,
                "Produto {ProductId} excluído do SQL Server, mas sua projeção no MongoDB não foi removida.",
                id);
        }
    }

    public async Task<PagedResult<ProductReadModel>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var filter = Builders<ProductDocument>.Filter.Empty;
        var totalItems = await _products.CountDocumentsAsync(
            filter,
            cancellationToken: cancellationToken);

        var documents = await _products
            .Find(filter)
            .SortBy(product => product.Name)
            .ThenBy(product => product.Id)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductReadModel>(
            documents.Select(Map).ToArray(),
            totalItems,
            page,
            pageSize);
    }

    private static ProductReadModel Map(ProductDocument document) =>
        new(document.Id, document.Name, document.Price);
}
