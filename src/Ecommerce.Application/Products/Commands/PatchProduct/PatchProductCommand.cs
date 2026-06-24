using MediatR;

namespace Ecommerce.Application.Products.Commands.PatchProduct;

public sealed record PatchProductCommand(
    Guid Id,
    string? Name,
    decimal? Price) : IRequest<bool>;
