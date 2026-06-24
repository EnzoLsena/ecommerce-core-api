using MediatR;

namespace Ecommerce.Application.Orders.Commands.ProcessOrder;

public sealed record ProcessOrderCommand(Guid Id) : IRequest<bool>;
