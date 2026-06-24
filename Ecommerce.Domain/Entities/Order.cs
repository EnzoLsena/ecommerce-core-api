using Ecommerce.Domain.Enums;
using Ecommerce.Domain.Exceptions;

namespace Ecommerce.Domain.Entities;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    private Order()
    {
    }

    public Order(Guid customerId)
    {
        EnsureValidId(customerId, "Customer is required.");

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Started;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public Customer Customer { get; private set; } = null!;

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        EnsureCanBeChanged();
        EnsureValidId(productId, "Product is required.");

        if (quantity <= 0)
            throw new DomainException("Item quantity must be greater than zero.");

        var existingItem = _items.SingleOrDefault(item => item.ProductId == productId);

        if (existingItem is null)
            _items.Add(new OrderItem(Id, productId, quantity, unitPrice));
        else
            existingItem.Change(existingItem.Quantity + quantity, unitPrice);

        Touch();
    }

    public void UpdateCustomer(Guid customerId)
    {
        EnsureCanBeChanged();
        EnsureValidId(customerId, "Customer is required.");

        CustomerId = customerId;
        Touch();
    }

    public void ChangeItem(Guid productId, int quantity, decimal unitPrice)
    {
        EnsureCanBeChanged();

        var item = _items.SingleOrDefault(item => item.ProductId == productId)
            ?? throw new DomainException("Order item was not found.");

        item.Change(quantity, unitPrice);
        Touch();
    }

    public void Cancel()
    {
        if (Status is not (OrderStatus.Started or OrderStatus.Processed))
            throw new DomainException("Only started or processed orders can be canceled.");

        Status = OrderStatus.Canceled;
        Touch();
    }

    public void MarkAsProcessed()
    {
        if (Status != OrderStatus.Started)
            throw new DomainException("Only started orders can be marked as processed.");

        Status = OrderStatus.Processed;
        Touch();
    }

    public void MarkAsShipped()
    {
        if (Status != OrderStatus.Processed)
            throw new DomainException("Only processed orders can be shipped.");

        Status = OrderStatus.Shipped;
        Touch();
    }

    public bool CanBeChanged() => Status == OrderStatus.Started;

    public void Delete()
    {
        if (DeletedAt.HasValue)
            return;

        DeletedAt = DateTime.UtcNow;

        foreach (var item in _items)
            item.Delete();
    }

    private void EnsureCanBeChanged()
    {
        if (!CanBeChanged())
            throw new DomainException("Only started orders can be changed.");
    }

    private static void EnsureValidId(Guid id, string message)
    {
        if (id == Guid.Empty)
            throw new DomainException(message);
    }

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}
