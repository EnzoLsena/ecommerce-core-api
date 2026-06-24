using MediatR;

namespace Ecommerce.Application.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : IRequest<bool>;
