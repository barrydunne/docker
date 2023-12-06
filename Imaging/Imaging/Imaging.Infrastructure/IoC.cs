using FluentFTP;
using Imaging.Application.ExternalApi;
using Imaging.Infrastructure.ExternalApi.Dummy;
using Imaging.Infrastructure.Metrics;
using Microservices.Shared.CloudFiles;
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
            .AddTransient<ICloudFiles, FtpFiles>()
            .AddTransient<IAsyncFtpClient, AsyncFtpClient>()
            .Configure<FtpFilesOptions>(configuration.GetSection("FtpFilesOptions"));

        // Metrics
        services
            .RegisterMetrics();

        return services;
    }
}
