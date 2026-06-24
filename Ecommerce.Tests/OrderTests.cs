using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
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

        order.ChangeItem(firstProduct, 2, 5m);

        order.TotalItems.Should().Be(5);
        order.TotalAmount.Should().Be(22m);
    }

    [Fact]
    public void ProcessAndCancel_RecordsLifecycleDates()
    {
        var order = new Order(Guid.NewGuid());

        order.MarkAsProcessed();
        order.Cancel();

        order.Status.Should().Be(OrderStatus.Canceled);
        order.PaidAt.Should().NotBeNull();
        order.CanceledAt.Should().NotBeNull();
        order.Code.Should().HaveLength(32);
    }
}
