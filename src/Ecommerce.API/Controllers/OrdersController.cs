using Ecommerce.API.Dtos.Orders;
using Ecommerce.Application.Common.Models;
using Ecommerce.Application.Orders.Commands.AddOrderItem;
using Ecommerce.Application.Orders.Commands.CancelOrder;
using Ecommerce.Application.Orders.Commands.ChangeOrderItem;
using Ecommerce.Application.Orders.Commands.CreateOrder;
using Ecommerce.Application.Orders.Commands.DeleteOrder;
using Ecommerce.Application.Orders.Commands.ProcessOrder;
using Ecommerce.Application.Orders.Commands.ShipOrder;
using Ecommerce.Application.Orders.Commands.UpdateOrderCustomer;
using Ecommerce.Application.Orders.Queries.GetOrderById;
using Ecommerce.Application.Orders.Queries.GetOrders;
using Ecommerce.Application.Orders.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/v1/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new CreateOrderCommand(
                request.CustomerId,
                (request.Items ?? [])
                    .Select(item => new CreateOrderItem(item.ProductId, item.Quantity))
                    .ToArray()),
            cancellationToken);

        return id.HasValue
            ? CreatedAtAction(
                nameof(GetById),
                new { id = id.Value },
                new CreateOrderResponse(id.Value))
            : NotFound();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderReadModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderReadModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetOrdersRequest request,
        CancellationToken cancellationToken)
    {
        var orders = await _mediator.Send(
            new GetOrdersQuery(
                request.Page,
                request.PageSize,
                request.Code,
                request.MinTotalAmount,
                request.MaxTotalAmount,
                request.MinTotalItems,
                request.MaxTotalItems,
                request.PaidFrom,
                request.PaidTo,
                request.CanceledFrom,
                request.CanceledTo),
            cancellationToken);

        return Ok(orders);
    }

    [HttpPatch("{id:guid}/customer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCustomer(
        Guid id,
        [FromBody] UpdateOrderCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(
            new UpdateOrderCustomerCommand(id, request.CustomerId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(
        Guid id,
        [FromBody] AddOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(
            new AddOrderItemCommand(id, request.ProductId, request.Quantity),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/items/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeItem(
        Guid id,
        Guid productId,
        [FromBody] ChangeOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(
            new ChangeOrderItemCommand(id, productId, request.Quantity),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}/process")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Process(Guid id, CancellationToken cancellationToken) =>
        ExecuteTransition(new ProcessOrderCommand(id), cancellationToken);

    [HttpPatch("{id:guid}/ship")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Ship(Guid id, CancellationToken cancellationToken) =>
        ExecuteTransition(new ShipOrderCommand(id), cancellationToken);

    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken) =>
        ExecuteTransition(new CancelOrderCommand(id), cancellationToken);

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteOrderCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private async Task<IActionResult> ExecuteTransition(
        IRequest<bool> command,
        CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }
}
