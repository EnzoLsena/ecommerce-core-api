using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ecommerce.Infrastructure.Persistence.Mongo.Documents;

internal sealed class CustomerDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
