using Ecommerce.Domain.Exceptions;

namespace Ecommerce.Domain.Entities;

public sealed class OrderItem
{
    private OrderItem()
    {
    }

    internal OrderItem(Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        Validate(quantity, unitPrice);

        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total => Quantity * UnitPrice;
    public DateTime? DeletedAt { get; private set; }

    public Order Order { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    internal void Change(int quantity, decimal unitPrice)
    {
        Validate(quantity, unitPrice);
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    internal void Delete() => DeletedAt ??= DateTime.UtcNow;

    private static void Validate(int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new DomainException("Item quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new DomainException("Item unit price cannot be negative.");
    }
}
