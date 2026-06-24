using MediatR;

namespace Ecommerce.Application.Orders.Commands.AddOrderItem;

public sealed record AddOrderItemCommand(
    Guid OrderId,
    Guid ProductId,
    int Quantity) : IRequest<bool>;
