using Email.Application.Repositories;
using Email.Application.Templates;
using Email.Infrastructure.Metrics;
using Email.Infrastructure.Repositories;
using Email.Infrastructure.Templates;
using Microservices.Shared.CloudEmail.Aws;
using Microservices.Shared.CloudEmail.Smtp;
using Microservices.Shared.CloudFiles.Aws;
using Microservices.Shared.CloudFiles.Ftp;
using Microservices.Shared.CloudSecrets;
using Microsoft.Extensions.Configuration;
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
            .AddCloudServices(builder.Configuration);

        // Metrics
        services
            .RegisterMetrics();

        // HTTP
        services
            .AddHttpClient();

        return services;
    }

    private static IServiceCollection AddCloudServices(this IServiceCollection services, IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("Microservices.CloudProvider") == "AWS"
            ? services.AddAwsCloudServices(configuration)
            : services.AddLocalCloudServices(configuration);
    }

    private static IServiceCollection AddAwsCloudServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddCloudEmailAws(configuration)
            .AddCloudFilesAws(configuration);
    }

    private static IServiceCollection AddLocalCloudServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddCloudEmailSmtp(configuration)
            .AddCloudFilesFtp(configuration);
    }
}
