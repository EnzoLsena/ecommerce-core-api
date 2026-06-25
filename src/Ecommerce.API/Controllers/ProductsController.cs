using Ecommerce.API.Requests.Products;
using Ecommerce.API.Responses.Products;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Products.Commands.CreateProduct;
using Ecommerce.Application.Products.Commands.DeleteProduct;
using Ecommerce.Application.Products.Commands.PatchProduct;
using Ecommerce.Application.Products.Commands.UpdateProduct;
using Ecommerce.Application.Products.Queries.GetProductById;
using Ecommerce.Application.Products.Queries.GetProducts;
using Ecommerce.Application.Products.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/v1/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateProductResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new CreateProductCommand(request.Name, request.Price),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new CreateProductResponse(id));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductReadModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductReadModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetProductsRequest request,
        CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(
            new GetProductsQuery(request.Page, request.PageSize),
            cancellationToken);

        return Ok(products);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(
            new UpdateProductCommand(id, request.Name, request.Price),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Patch(
        Guid id,
        [FromBody] PatchProductRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(
            new PatchProductCommand(id, request.Name, request.Price),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(
            new DeleteProductCommand(id),
            cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
