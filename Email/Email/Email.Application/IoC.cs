using AspNet.KickStarter.CQRS;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Email.Application;

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
    /// <returns>The services instance.</returns>
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        // CQRS
        services
            .AddMediatR(_ => _.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddTracePipelineBehavior()
            .AddValidationPipelineBehavior()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

        return services;
    }
}
