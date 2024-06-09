using Imaging.Application.ExternalApi;
using Imaging.Infrastructure.ExternalApi.Dummy;
using Imaging.Infrastructure.Metrics;
using Microservices.Shared.CloudFiles.Aws;
using Microservices.Shared.CloudFiles.Ftp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Imaging.Infrastructure;

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
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
    {
        // External service
        services
            .AddTransient<IExternalApi, DummyApi>(); // Replace DummyApi with BingApi for real images

        // Cloud Services
        services
            .AddCloudServices(configuration);

        // Metrics
        services
            .RegisterMetrics();

        return services;
    }

    private static IServiceCollection AddCloudServices(this IServiceCollection services, IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("Microservices.CloudProvider") == "AWS"
            ? services.AddAwsCloudServices(configuration)
            : services.AddLocalCloudServices(configuration);
    }

    private static IServiceCollection AddAwsCloudServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddCloudFilesAws(configuration);

    private static IServiceCollection AddLocalCloudServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddCloudFilesFtp(configuration);
}
