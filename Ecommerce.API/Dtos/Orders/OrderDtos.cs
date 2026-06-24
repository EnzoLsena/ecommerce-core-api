namespace Ecommerce.API.Dtos.Orders;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    IReadOnlyCollection<CreateOrderItemRequest>? Items);

public sealed record CreateOrderItemRequest(Guid ProductId, int Quantity);

public sealed record CreateOrderResponse(Guid Id);

public sealed record UpdateOrderCustomerRequest(Guid CustomerId);

public sealed record AddOrderItemRequest(Guid ProductId, int Quantity);

public sealed record ChangeOrderItemRequest(int Quantity);

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
