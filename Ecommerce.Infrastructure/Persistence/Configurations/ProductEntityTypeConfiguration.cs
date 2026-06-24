using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public sealed class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.HasQueryFilter(product => product.DeletedAt == null);

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasMany(product => product.OrderItems)
            .WithOne(item => item.Product)
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(product => product.OrderItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
