using FluentFTP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.CloudFiles.Ftp;

/// <summary>
/// Provides extensions for dependency injection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    /// <summary>
    /// Add the FtpFiles type as the ICloudFiles service.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="configSectionName">The name of the configuration section to use to create the required FtpFilesOptions, defaults to "FtpFilesOptions".</param>
    /// <returns>The original builder.</returns>
    public static IServiceCollection AddCloudFilesFtp(this IServiceCollection services, IConfiguration configuration, string configSectionName = "FtpFilesOptions")
    {
        return services
            .AddTransient<ICloudFiles, FtpFiles>()
            .AddTransient<IAsyncFtpClient, AsyncFtpClient>()
            .Configure<FtpFilesOptions>(configuration.GetSection(configSectionName));
    }
}
