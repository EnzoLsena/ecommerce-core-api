using Ecommerce.Application;
using Ecommerce.API.ExceptionHandling;
using Ecommerce.Infrastructure;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi("v1");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/{documentName}.json");
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Ecommerce API v1");
    });
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var requestId = context.Request.Headers.TryGetValue("X-Request-Id", out var headerRequestId)
        && !string.IsNullOrWhiteSpace(headerRequestId.ToString())
            ? headerRequestId.ToString()
            : context.TraceIdentifier;

    var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

    context.Response.Headers["X-Request-Id"] = requestId;

    using (LogContext.PushProperty("RequestId", requestId))
    using (LogContext.PushProperty("TraceId", traceId))
    {
        await next();
    }
});

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
