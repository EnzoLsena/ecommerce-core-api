using MediatR;

namespace Ecommerce.Application.Customers.Commands.PatchCustomer;

public sealed record PatchCustomerCommand(
    Guid Id,
    string? Name,
    string? Email) : IRequest<bool>;
