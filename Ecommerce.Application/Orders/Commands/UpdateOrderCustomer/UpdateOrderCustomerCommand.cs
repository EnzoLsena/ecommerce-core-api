using MediatR;

namespace Ecommerce.Application.Orders.Commands.UpdateOrderCustomer;

public sealed record UpdateOrderCustomerCommand(Guid Id, Guid CustomerId) : IRequest<bool>;
