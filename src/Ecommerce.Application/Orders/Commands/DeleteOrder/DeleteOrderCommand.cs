using MediatR;

namespace Ecommerce.Application.Orders.Commands.DeleteOrder;

public sealed record DeleteOrderCommand(Guid Id) : IRequest<bool>;
