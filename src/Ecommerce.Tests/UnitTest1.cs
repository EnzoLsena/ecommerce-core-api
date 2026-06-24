using AutoMapper;
using Ecommerce.Application;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ecommerce.Tests;

public sealed class MappingTests
{
    [Fact]
    public void AutoMapperConfigurationShouldBeValid()
    {
        var configuration = new MapperConfiguration(
            options => options.AddMaps(typeof(DependencyInjection).Assembly),
            NullLoggerFactory.Instance);

        configuration.AssertConfigurationIsValid();
    }
}
