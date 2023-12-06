using Email.Application.Repositories;
using Email.Application.Templates;
using Email.Infrastructure.Metrics;
using Email.Infrastructure.Repositories;
using Email.Infrastructure.Templates;
using FluentFTP;
using Microservices.Shared.CloudEmail;
using Microservices.Shared.CloudEmail.Smtp;
using Microservices.Shared.CloudFiles;
using Microservices.Shared.CloudFiles.Ftp;
using Microservices.Shared.CloudSecrets.SecretsManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Email.Infrastructure;

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
    /// <param name="builder">The builder that has the ICloudSecrets service already registered.</param>
    /// <returns>The services instance.</returns>
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        // Repositories
        services
            .AddSingleton<IEmailRepository, EmailRepository>()
            .AddMySqlDbContext<EmailRepositoryDbContext>(builder.Configuration.GetSection("ConnectionStrings").ApplySecret(builder, "mysql", "email", "mysql.connectionstring")["mysql"]!);

        // Email generator
        services
            .AddTransient<ITemplateEngine, TemplateEngine>();

        // Cloud Services
        services
            .AddTransient<ICloudEmail, SmtpEmail>()
            .AddTransient<ISmtpClient, SmtpClientAdapter>()
            .Configure<SmtpEmailOptions>(builder.Configuration.GetSection("SmtpEmailOptions"))
            .AddTransient<ICloudFiles, FtpFiles>()
            .AddTransient<IAsyncFtpClient, AsyncFtpClient>()
            .Configure<FtpFilesOptions>(builder.Configuration.GetSection("FtpFilesOptions"));

        // Metrics
        services
            .RegisterMetrics();

        // HTTP
        services
            .AddHttpClient();

        return services;
    }
}
