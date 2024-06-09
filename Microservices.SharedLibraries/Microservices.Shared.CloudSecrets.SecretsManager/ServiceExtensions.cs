using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.CloudSecrets.SecretsManager;

/// <summary>
/// Provides extensions for dependency injection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    /// <summary>
    /// Add the SecretsManagerSecrets type as the ICloudSecrets service.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="configSectionName">The name of the configuration section to use to create the required SecretsManagerOptions, defaults to "SecretsManagerOptions".</param>
    /// <returns>The original builder.</returns>
    public static IServiceCollection AddCloudSecretsManager(this IServiceCollection services, IConfiguration configuration, string configSectionName = "SecretsManagerOptions")
    {
        return services
            .AddTransient<ICloudSecrets, SecretsManagerSecrets>()
            .Configure<SecretsManagerOptions>(configuration.GetSection(configSectionName));
    }
}
