using MediatR;

namespace Ecommerce.Application.Customers.Commands.DeleteCustomer;

public sealed record DeleteCustomerCommand(Guid Id) : IRequest<bool>;
