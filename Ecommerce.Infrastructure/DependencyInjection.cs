using Ecommerce.Application.Products.Abstractions;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Persistence.Mongo;
using Ecommerce.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Ecommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not configured.");

        services.AddDbContext<EcommerceDbContext>(options =>
            options.UseSqlServer(connectionString));

        var mongoConnectionString = configuration["MongoDb:ConnectionString"]
            ?? throw new InvalidOperationException(
                "MongoDB connection string was not configured.");

        var mongoDatabaseName = configuration["MongoDb:DatabaseName"]
            ?? throw new InvalidOperationException(
                "MongoDB database name was not configured.");

        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
        services.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IMongoClient>()
                .GetDatabase(mongoDatabaseName));

        services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
        services.AddScoped<IProductReadStore, MongoProductReadStore>();

        return services;
    }
}
