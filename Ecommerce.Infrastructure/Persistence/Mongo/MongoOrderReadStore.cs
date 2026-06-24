using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Orders.Abstractions;
using Ecommerce.Application.Orders.Models;
using Ecommerce.Infrastructure.Persistence.Mongo.Documents;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Ecommerce.Infrastructure.Persistence.Mongo;

public sealed class MongoOrderReadStore(
    IMongoDatabase database,
    ILogger<MongoOrderReadStore> logger) : IOrderReadStore
{
    private readonly IMongoCollection<OrderDocument> _orders =
        database.GetCollection<OrderDocument>("orders");

    public async Task TryUpsertAsync(
        OrderReadModel order,
        CancellationToken cancellationToken)
    {
        var document = Map(order);

        try
        {
            await _orders.ReplaceOneAsync(
                item => item.Id == order.Id,
                document,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken);
        }
        catch (MongoException exception)
        {
            logger.LogWarning(
                exception,
                "Pedido {OrderId} salvo no SQL Server, mas sua projeção no MongoDB falhou.",
                order.Id);
        }
    }

    public async Task TryDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _orders.DeleteOneAsync(order => order.Id == id, cancellationToken);
        }
        catch (MongoException exception)
        {
            logger.LogWarning(
                exception,
                "Pedido {OrderId} excluído do SQL Server, mas sua projeção no MongoDB não foi removida.",
                id);
        }
    }

    public async Task<OrderReadModel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var document = await _orders
            .Find(order => order.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document is null ? null : Map(document);
    }

    public async Task<PagedResult<OrderReadModel>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var filter = Builders<OrderDocument>.Filter.Empty;
        var totalItems = await _orders.CountDocumentsAsync(
            filter,
            cancellationToken: cancellationToken);

        var documents = await _orders
            .Find(filter)
            .SortByDescending(order => order.CreatedAt)
            .ThenBy(order => order.Id)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<OrderReadModel>(
            documents.Select(Map).ToArray(),
            totalItems,
            page,
            pageSize);
    }

    private static OrderDocument Map(OrderReadModel order) =>
        new()
        {
            Id = order.Id,
            Customer = new OrderCustomerDocument
            {
                Id = order.Customer.Id,
                Name = order.Customer.Name,
                Email = order.Customer.Email
            },
            Status = order.Status,
            Items = order.Items.Select(item => new OrderItemDocument
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Total = item.Total
            }).ToArray(),
            Total = order.Total,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };

    private static OrderReadModel Map(OrderDocument document) =>
        new(
            document.Id,
            new OrderCustomerReadModel(
                document.Customer.Id,
                document.Customer.Name,
                document.Customer.Email),
            document.Status,
            document.Items.Select(item => new OrderItemReadModel(
                item.Id,
                item.ProductId,
                item.ProductName,
                item.Quantity,
                item.UnitPrice,
                item.Total)).ToArray(),
            document.Total,
            document.CreatedAt,
            document.UpdatedAt);
}
