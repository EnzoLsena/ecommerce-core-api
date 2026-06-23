using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public sealed class EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
    : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        new CustomerEntityTypeConfiguration().Configure(modelBuilder.Entity<Customer>());
        new ProductEntityTypeConfiguration().Configure(modelBuilder.Entity<Product>());
        new OrderEntityTypeConfiguration().Configure(modelBuilder.Entity<Order>());
        new OrderItemEntityTypeConfiguration().Configure(modelBuilder.Entity<OrderItem>());
    }
}
