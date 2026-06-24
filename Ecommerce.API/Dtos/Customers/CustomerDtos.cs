namespace Ecommerce.API.Dtos.Customers;

public sealed record CreateCustomerRequest(string Name, string Email);

public sealed record CreateCustomerResponse(Guid Id);

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
