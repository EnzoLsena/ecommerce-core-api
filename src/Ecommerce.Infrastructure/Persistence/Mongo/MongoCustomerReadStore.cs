using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Customers.Abstractions;
using Ecommerce.Application.Customers.Models;
using Ecommerce.Infrastructure.Persistence.Mongo.Documents;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Ecommerce.Infrastructure.Persistence.Mongo;

public sealed class MongoCustomerReadStore(
    IMongoDatabase database,
    ILogger<MongoCustomerReadStore> logger) : ICustomerReadStore
{
    private readonly IMongoCollection<CustomerDocument> _customers =
        database.GetCollection<CustomerDocument>("customers");

    public async Task TryUpsertAsync(
        CustomerReadModel customer,
        CancellationToken cancellationToken)
    {
        var document = new CustomerDocument
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email
        };

        try
        {
            await _customers.ReplaceOneAsync(
                item => item.Id == customer.Id,
                document,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken);
        }
        catch (MongoException exception)
        {
            logger.LogWarning(
                exception,
                "Cliente {CustomerId} salvo no SQL Server, mas sua projeção no MongoDB falhou.",
                customer.Id);
        }
    }

    public async Task TryDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _customers.DeleteOneAsync(
                customer => customer.Id == id,
                cancellationToken);
        }
        catch (MongoException exception)
        {
            logger.LogWarning(
                exception,
                "Cliente {CustomerId} excluído do SQL Server, mas sua projeção no MongoDB não foi removida.",
                id);
        }
    }

    public async Task<CustomerReadModel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var document = await _customers
            .Find(customer => customer.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document is null ? null : Map(document);
    }

    public async Task<PagedResult<CustomerReadModel>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var filter = Builders<CustomerDocument>.Filter.Empty;
        var totalItems = await _customers.CountDocumentsAsync(
            filter,
            cancellationToken: cancellationToken);

        var documents = await _customers
            .Find(filter)
            .SortBy(customer => customer.Name)
            .ThenBy(customer => customer.Id)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CustomerReadModel>(
            documents.Select(Map).ToArray(),
            totalItems,
            page,
            pageSize);
    }

    private static CustomerReadModel Map(CustomerDocument document) =>
        new(document.Id, document.Name, document.Email);
}
