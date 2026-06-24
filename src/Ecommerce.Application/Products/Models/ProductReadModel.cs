namespace Ecommerce.Application.Products.Models;

public sealed record ProductReadModel(
    Guid Id,
    string Name,
    decimal Price);
