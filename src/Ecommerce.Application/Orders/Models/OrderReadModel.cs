using Ecommerce.Domain.Enums;

namespace Ecommerce.Application.Orders.Models;

public sealed record OrderReadModel(
    Guid Id,
    OrderCustomerReadModel Customer,
    OrderStatus Status,
    IReadOnlyCollection<OrderItemReadModel> Items,
    decimal TotalAmount,
    int TotalItems,
    string Code,
    DateTime? PaidAt,
    DateTime? CanceledAt,
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
