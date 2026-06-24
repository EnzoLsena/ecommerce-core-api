using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ecommerce.Infrastructure.Persistence.Mongo.Documents;

internal sealed class ProductDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; init; }
}
