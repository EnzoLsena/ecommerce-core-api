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
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/v1/orders")]
public sealed class OrdersController(ISender mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var id = await mediator.Send(
            new CreateOrderCommand(request.CustomerId),
            cancellationToken);

        return id.HasValue
            ? CreatedAtAction(nameof(GetById), new { id = id.Value }, new { id = id.Value })
            : NotFound();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetOrdersRequest request,
        CancellationToken cancellationToken)
    {
        var orders = await mediator.Send(
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
    public async Task<IActionResult> UpdateCustomer(
        Guid id,
        UpdateOrderCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(
            new UpdateOrderCustomerCommand(id, request.CustomerId),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(
        Guid id,
        AddOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(
            new AddOrderItemCommand(id, request.ProductId, request.Quantity, request.UnitPrice),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/items/{productId:guid}")]
    public async Task<IActionResult> ChangeItem(
        Guid id,
        Guid productId,
        ChangeOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(
            new ChangeOrderItemCommand(id, productId, request.Quantity, request.UnitPrice),
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}/process")]
    public Task<IActionResult> Process(Guid id, CancellationToken cancellationToken) =>
        ExecuteTransition(new ProcessOrderCommand(id), cancellationToken);

    [HttpPatch("{id:guid}/ship")]
    public Task<IActionResult> Ship(Guid id, CancellationToken cancellationToken) =>
        ExecuteTransition(new ShipOrderCommand(id), cancellationToken);

    [HttpPatch("{id:guid}/cancel")]
    public Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken) =>
        ExecuteTransition(new CancelOrderCommand(id), cancellationToken);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await mediator.Send(new DeleteOrderCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private async Task<IActionResult> ExecuteTransition(
        IRequest<bool> command,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }
}

public sealed record CreateOrderRequest(Guid CustomerId);

public sealed record UpdateOrderCustomerRequest(Guid CustomerId);

public sealed record AddOrderItemRequest(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice);

public sealed record ChangeOrderItemRequest(
    int Quantity,
    decimal UnitPrice);

public sealed class GetOrdersRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Code { get; init; }
    public decimal? MinTotalAmount { get; init; }
    public decimal? MaxTotalAmount { get; init; }
    public int? MinTotalItems { get; init; }
    public int? MaxTotalItems { get; init; }
    public DateTime? PaidFrom { get; init; }
    public DateTime? PaidTo { get; init; }
    public DateTime? CanceledFrom { get; init; }
    public DateTime? CanceledTo { get; init; }
}
