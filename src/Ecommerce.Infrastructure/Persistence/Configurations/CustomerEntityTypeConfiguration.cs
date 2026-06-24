using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public sealed class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(customer => customer.Id);

        builder.HasQueryFilter(customer => customer.DeletedAt == null);

        builder.Property(customer => customer.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(customer => customer.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.HasIndex(customer => customer.Email)
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");

        builder.HasMany(customer => customer.Orders)
            .WithOne(order => order.Customer)
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(customer => customer.Orders)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
