using Ecommerce.Application.Products.Models;
using MediatR;

namespace Ecommerce.Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductReadModel?>;
