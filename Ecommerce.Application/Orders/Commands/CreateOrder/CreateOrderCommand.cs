using MediatR;

namespace Ecommerce.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CustomerId,
    IReadOnlyCollection<CreateOrderItem> Items) : IRequest<Guid?>;

public sealed record CreateOrderItem(Guid ProductId, int Quantity);
