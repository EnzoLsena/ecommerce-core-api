using MediatR;

namespace Ecommerce.Application.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(string Name, string Email) : IRequest<Guid>;
