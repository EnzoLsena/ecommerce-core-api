using MediatR;

namespace Ecommerce.Application.Orders.Commands.ShipOrder;

public sealed record ShipOrderCommand(Guid Id) : IRequest<bool>;
