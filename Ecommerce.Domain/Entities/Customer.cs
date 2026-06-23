namespace Ecommerce.Domain.Entities;

public sealed class Customer
{
    private readonly List<Order> _orders = [];

    private Customer()
    {
    }

    public Customer(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Customer email is required.", nameof(email));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim();
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
}
