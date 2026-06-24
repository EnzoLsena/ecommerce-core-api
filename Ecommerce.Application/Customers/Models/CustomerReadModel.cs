namespace Ecommerce.Application.Customers.Models;

public sealed record CustomerReadModel(
    Guid Id,
    string Name,
    string Email);
