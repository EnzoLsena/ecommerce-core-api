using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public sealed class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(item => item.Id);

        builder.HasQueryFilter(item => item.DeletedAt == null);

        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Ignore(item => item.Total);

        builder.HasIndex(item => new { item.OrderId, item.ProductId })
            .IsUnique();
    }
}
