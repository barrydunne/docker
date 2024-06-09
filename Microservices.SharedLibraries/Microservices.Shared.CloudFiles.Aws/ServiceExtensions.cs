using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.CloudFiles.Aws;

/// <summary>
/// Provides extensions for dependency injection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    /// <summary>
    /// Add the AwsFiles type as the ICloudFiles service.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="configSectionName">The name of the configuration section to use to create the required AwsFiles, defaults to "AwsFilesOptions".</param>
    /// <returns>The original builder.</returns>
    public static IServiceCollection AddCloudFilesAws(this IServiceCollection services, IConfiguration configuration, string configSectionName = "AwsFilesOptions")
    {
        return services
            .AddDefaultAWSOptions(configuration.GetAWSOptions())
            .AddAWSService<IAmazonS3>()
            .AddTransient<ICloudFiles, AwsFiles>()
            .Configure<AwsFilesOptions>(configuration.GetSection(configSectionName));
    }
}
