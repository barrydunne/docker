using Amazon.SecretsManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.CloudSecrets.Aws;

/// <summary>
/// Provides extensions for dependency injection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    /// <summary>
    /// Add the AwsSecrets type as the ICloudSecrets service.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The original builder.</returns>
    public static IServiceCollection AddCloudSecretsAws(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDefaultAWSOptions(configuration.GetAWSOptions())
            .AddAWSService<IAmazonSecretsManager>()
            .AddTransient<ICloudSecrets, AwsSecrets>();
    }
}
