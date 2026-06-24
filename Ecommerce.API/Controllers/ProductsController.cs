using Ecommerce.Application.Products.Commands.CreateProduct;
using Ecommerce.Application.Products.Commands.DeleteProduct;
using Ecommerce.Application.Products.Commands.PatchProduct;
using Ecommerce.Application.Products.Commands.UpdateProduct;
using Ecommerce.Application.Products.Queries.GetProductById;
using Ecommerce.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/v1/products")]
public sealed class ProductsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var id = await mediator.Send(
            new CreateProductCommand(request.Name, request.Price),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await mediator.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetProductsRequest request,
        CancellationToken cancellationToken)
    {
        var products = await mediator.Send(
            new GetProductsQuery(request.Page, request.PageSize),
            cancellationToken);

        return Ok(products);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(
            new UpdateProductCommand(id, request.Name, request.Price),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Patch(
        Guid id,
        PatchProductRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(
            new PatchProductCommand(id, request.Name, request.Price),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await mediator.Send(
            new DeleteProductCommand(id),
            cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}

public sealed record CreateProductRequest(
    string Name,
    decimal Price);

public sealed record UpdateProductRequest(
    string Name,
    decimal Price);

public sealed record PatchProductRequest(
    string? Name,
    decimal? Price);

public sealed class GetProductsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
