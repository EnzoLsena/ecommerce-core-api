using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Products.Models;
using MediatR;

namespace Ecommerce.Application.Products.Queries.GetProducts;

public sealed record GetProductsQuery(int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<ProductReadModel>>;
