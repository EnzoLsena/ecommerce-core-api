namespace Ecommerce.API.Requests.Products;

public sealed record CreateProductRequest(string Name, decimal Price);

public sealed record UpdateProductRequest(string Name, decimal Price);

public sealed record PatchProductRequest(string? Name, decimal? Price);

public sealed class GetProductsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
