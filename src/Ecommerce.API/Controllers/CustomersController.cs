using Ecommerce.API.Requests.Customers;
using Ecommerce.API.Responses.Customers;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Customers.Commands.CreateCustomer;
using Ecommerce.Application.Customers.Commands.DeleteCustomer;
using Ecommerce.Application.Customers.Commands.PatchCustomer;
using Ecommerce.Application.Customers.Commands.UpdateCustomer;
using Ecommerce.Application.Customers.Queries.GetCustomerById;
using Ecommerce.Application.Customers.Queries.GetCustomers;
using Ecommerce.Application.Customers.Models;
using Ecommerce.Application.Orders.Models;
using Ecommerce.Application.Orders.Queries.GetOrdersByCustomer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/v1/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateCustomerResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new CreateCustomerCommand(request.Name, request.Email),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new CreateCustomerResponse(id));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerReadModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _mediator.Send(
            new GetCustomerByIdQuery(id),
            cancellationToken);

        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CustomerReadModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetCustomersRequest request,
        CancellationToken cancellationToken)
    {
        var customers = await _mediator.Send(
            new GetCustomersQuery(request.Page, request.PageSize),
            cancellationToken);

        return Ok(customers);
    }

    [HttpGet("{id:guid}/orders")]
    [ProducesResponseType(typeof(PagedResult<OrderReadModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrders(
        Guid id,
        [FromQuery] GetCustomerOrdersRequest request,
        CancellationToken cancellationToken)
    {
        var orders = await _mediator.Send(
            new GetOrdersByCustomerQuery(id, request.Page, request.PageSize),
            cancellationToken);

        return orders is null ? NotFound() : Ok(orders);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(
            new UpdateCustomerCommand(id, request.Name, request.Email),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Patch(
        Guid id,
        [FromBody] PatchCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(
            new PatchCustomerCommand(id, request.Name, request.Email),
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
            new DeleteCustomerCommand(id),
            cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
