using Amazon.SimpleEmail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.CloudEmail.Aws;

/// <summary>
/// Provides extensions for dependency injection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    /// <summary>
    /// Add the AwsEmail type as the ICloudEmail service.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="configSectionName">The name of the configuration section to use to create the required AwsEmailOptions, defaults to "AwsEmailOptions".</param>
    /// <returns>The original builder.</returns>
    public static IServiceCollection AddCloudEmailAws(this IServiceCollection services, IConfiguration configuration, string configSectionName = "AwsEmailOptions")
    {
        return services
            .AddDefaultAWSOptions(configuration.GetAWSOptions())
            .AddAWSService<IAmazonSimpleEmailService>()
            .AddTransient<ICloudEmail, AwsEmail>()
            .Configure<AwsEmailOptions>(configuration.GetSection(configSectionName));
    }
}
