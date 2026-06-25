namespace Ecommerce.API.Dtos.Orders;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    IReadOnlyCollection<CreateOrderItemRequest>? Items);

public sealed record CreateOrderItemRequest(Guid ProductId, int Quantity);

public sealed record CreateOrderResponse(Guid Id);

public sealed record UpdateOrderCustomerRequest(Guid CustomerId);

public sealed record AddOrderItemRequest(Guid ProductId, int Quantity);

public sealed record ChangeOrderItemRequest(int Quantity);
