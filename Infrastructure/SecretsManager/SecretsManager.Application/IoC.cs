using AspNet.KickStarter.CQRS;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SecretsManager.Application;

/// <summary>
/// Register services for dependency injection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class IoC
{
    /// <summary>
    /// Register services for dependency injection.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The application configuration manager.</param>
    /// <returns>The services instance.</returns>
    public static IServiceCollection RegisterApplication(this IServiceCollection services, IConfigurationManager configuration)
    {
        // CQRS
        services
            .AddMediatR(_ => _.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

        // Redis
        var conf = configuration.GetSection("Redis").Get<RedisConfiguration>() ?? throw new KeyNotFoundException("Redis configuration not found");
        services
            .AddStackExchangeRedisExtensions<NewtonsoftSerializer>(conf);

        return services;
    }
}
