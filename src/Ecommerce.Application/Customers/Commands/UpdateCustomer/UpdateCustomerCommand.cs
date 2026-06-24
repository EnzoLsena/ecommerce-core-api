using MediatR;

namespace Ecommerce.Application.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(Guid Id, string Name, string Email) : IRequest<bool>;
