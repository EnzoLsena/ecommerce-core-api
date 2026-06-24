using Ecommerce.Application.Customers.Models;
using MediatR;

namespace Ecommerce.Application.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerReadModel?>;
