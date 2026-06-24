using MediatR;

namespace Ecommerce.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(Guid CustomerId) : IRequest<Guid?>;
