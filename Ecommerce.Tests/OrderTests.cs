using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Domain.Exceptions;
using FluentAssertions;

namespace Ecommerce.Tests;

public sealed class OrderTests
{
    [Fact]
    public void AddAndChangeItems_RecalculatesTotals()
    {
        var order = new Order(Guid.NewGuid());
        var firstProduct = Guid.NewGuid();

        order.AddItem(firstProduct, 2, 10.50m);
        order.AddItem(Guid.NewGuid(), 3, 4m);
        order.AddItem(firstProduct, 1, 12m);

        order.TotalItems.Should().Be(6);
        order.TotalAmount.Should().Be(48m);

        order.ChangeItem(firstProduct, 2);

        order.TotalItems.Should().Be(5);
        order.TotalAmount.Should().Be(36m);
    }

    [Fact]
    public void ProcessAndCancel_RecordsLifecycleDates()
    {
        var order = new Order(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), 1, 10m);

        order.MarkAsProcessed();
        order.Cancel();

        order.Status.Should().Be(OrderStatus.Canceled);
        order.PaidAt.Should().NotBeNull();
        order.CanceledAt.Should().NotBeNull();
        order.Code.Should().HaveLength(32);
    }

    [Fact]
    public void Process_WithoutItems_IsRejected()
    {
        var order = new Order(Guid.NewGuid());

        var action = order.MarkAsProcessed;

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*at least one product*");
        order.Status.Should().Be(OrderStatus.Started);
    }

    [Fact]
    public void StartedOrder_CannotBeShipped()
    {
        var order = CreateOrderWithItem();

        var action = order.MarkAsShipped;

        action.Should().Throw<DomainException>();
        order.Status.Should().Be(OrderStatus.Started);
    }

    [Fact]
    public void ProcessedOrder_CanBeShipped_ButCannotBeChanged()
    {
        var order = CreateOrderWithItem();
        order.MarkAsProcessed();

        order.MarkAsShipped();
        var action = () => order.UpdateCustomer(Guid.NewGuid());

        order.Status.Should().Be(OrderStatus.Shipped);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void ShippedOrder_CannotBeCanceled()
    {
        var order = CreateOrderWithItem();
        order.MarkAsProcessed();
        order.MarkAsShipped();

        var action = order.Cancel;

        action.Should().Throw<DomainException>();
        order.Status.Should().Be(OrderStatus.Shipped);
    }

    [Fact]
    public void CanceledOrder_CannotBeChangedProcessedOrShipped()
    {
        var order = CreateOrderWithItem();
        order.Cancel();

        var change = () => order.AddItem(Guid.NewGuid(), 1, 5m);
        var process = order.MarkAsProcessed;
        var ship = order.MarkAsShipped;

        change.Should().Throw<DomainException>();
        process.Should().Throw<DomainException>();
        ship.Should().Throw<DomainException>();
        order.Status.Should().Be(OrderStatus.Canceled);
    }

    private static Order CreateOrderWithItem()
    {
        var order = new Order(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), 1, 10m);
        return order;
    }
}
