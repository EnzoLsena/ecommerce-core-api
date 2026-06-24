using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Orders.Models;

public sealed record OrderReadModel(
    Guid Id,
    OrderCustomerReadModel Customer,
    OrderStatus Status,
    IReadOnlyCollection<OrderItemReadModel> Items,
    decimal Total,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record OrderCustomerReadModel(
    Guid Id,
    string Name,
    string Email);

public sealed record OrderItemReadModel(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Total);
