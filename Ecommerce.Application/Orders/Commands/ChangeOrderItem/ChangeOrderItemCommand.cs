using MediatR;

namespace Ecommerce.Application.Orders.Commands.ChangeOrderItem;

public sealed record ChangeOrderItemCommand(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice) : IRequest<bool>;
