using Ecommerce.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ecommerce.Infrastructure.Persistence.Mongo.Documents;

internal sealed class OrderDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; }

    public OrderCustomerDocument Customer { get; init; } = null!;

    [BsonRepresentation(BsonType.String)]
    public OrderStatus Status { get; init; }

    public IReadOnlyCollection<OrderItemDocument> Items { get; init; } = [];

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Total { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

internal sealed class OrderCustomerDocument
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

internal sealed class OrderItemDocument
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; }

    [BsonRepresentation(BsonType.String)]
    public Guid ProductId { get; init; }

    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal UnitPrice { get; init; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Total { get; init; }
}
