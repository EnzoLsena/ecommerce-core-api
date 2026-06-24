using Ecommerce.Application.Customers.Commands.CreateCustomer;
using Ecommerce.Application.Customers.Commands.DeleteCustomer;
using Ecommerce.Application.Customers.Commands.PatchCustomer;
using Ecommerce.Application.Customers.Commands.UpdateCustomer;
using Ecommerce.Application.Customers.Queries.GetCustomerById;
using Ecommerce.Application.Customers.Queries.GetCustomers;
using Ecommerce.Application.Orders.Queries.GetOrdersByCustomer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/v1/customers")]
public sealed class CustomersController(ISender mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var id = await mediator.Send(
            new CreateCustomerCommand(request.Name, request.Email),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await mediator.Send(
            new GetCustomerByIdQuery(id),
            cancellationToken);

        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetCustomersRequest request,
        CancellationToken cancellationToken)
    {
        var customers = await mediator.Send(
            new GetCustomersQuery(request.Page, request.PageSize),
            cancellationToken);

        return Ok(customers);
    }

    [HttpGet("{id:guid}/orders")]
    public async Task<IActionResult> GetOrders(
        Guid id,
        [FromQuery] GetCustomerOrdersRequest request,
        CancellationToken cancellationToken)
    {
        var orders = await mediator.Send(
            new GetOrdersByCustomerQuery(id, request.Page, request.PageSize),
            cancellationToken);

        return orders is null ? NotFound() : Ok(orders);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(
            new UpdateCustomerCommand(id, request.Name, request.Email),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Patch(
        Guid id,
        PatchCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(
            new PatchCustomerCommand(id, request.Name, request.Email),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await mediator.Send(
            new DeleteCustomerCommand(id),
            cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}

public sealed record CreateCustomerRequest(string Name, string Email);

public sealed record UpdateCustomerRequest(string Name, string Email);

public sealed record PatchCustomerRequest(string? Name, string? Email);

public sealed class GetCustomersRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class GetCustomerOrdersRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
