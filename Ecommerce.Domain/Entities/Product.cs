using Ecommerce.Domain.Exceptions;

namespace Ecommerce.Domain.Entities;

public sealed class Product
{
    private readonly List<OrderItem> _orderItems = [];

    private Product()
    {
    }

    public Product(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));

        if (price < 0)
            throw new DomainException("Product price cannot be negative.");

        Id = Guid.NewGuid();
        Name = name.Trim();
        Price = price;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
}
