using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.CloudEmail.Smtp;

/// <summary>
/// Provides extensions for dependency injection.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    /// <summary>
    /// Add the SmtpEmail type as the ICloudEmail service.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="configSectionName">The name of the configuration section to use to create the required SmtpEmailOptions, defaults to "SmtpEmailOptions".</param>
    /// <returns>The original builder.</returns>
    public static IServiceCollection AddCloudEmailSmtp(this IServiceCollection services, IConfiguration configuration, string configSectionName = "SmtpEmailOptions")
    {
        return services
            .AddTransient<ICloudEmail, SmtpEmail>()
            .AddTransient<ISmtpClient, SmtpClientAdapter>()
            .Configure<SmtpEmailOptions>(configuration.GetSection(configSectionName));
    }
}
