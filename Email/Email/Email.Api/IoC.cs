using Email.Api.HttpHandlers;
using Email.Logic.Commands;
using Email.Logic.Templates;
using Email.Repository;
using FluentFTP;
using Microservices.Shared.CloudEmail;
using Microservices.Shared.CloudEmail.Smtp;
using Microservices.Shared.CloudFiles;
using Microservices.Shared.CloudFiles.Ftp;
using Microservices.Shared.Utilities;

namespace Email.Api
{
    /// <summary>
    /// Register services for dependency injection.
    /// </summary>
    internal static class IoC
    {
        /// <summary>
        /// Register services for dependency injection.
        /// </summary>
        /// <param name="builder">The builder to register services with.</param>
        internal static void RegisterServices(WebApplicationBuilder builder)
        {
            // Default microservice dependencies
            builder.AddMicroserviceDependencies();

            // API Handlers
            builder.Services
                .AddTransient<EmailHandler>()
                .AddTransient<SearchHandler>();

            // Repositories
            builder.Services
                .AddSingleton<IEmailRepository, EmailRepository>();

            // CQRS
            builder.Services
                .AddMediatR(_ => _.RegisterServicesFromAssembly(typeof(SendEmailCommand).Assembly));

            // Email generator
            builder.Services
                .AddTransient<ITemplateEngine, TemplateEngine>();

            // Cloud Services
            builder.Services
                .AddTransient<ICloudEmail, SmtpEmail>()
                .AddTransient<ISmtpClient, SmtpClientAdapter>()
                .Configure<SmtpEmailOptions>(builder.Configuration.GetSection("SmtpEmailOptions"))
                .AddTransient<ICloudFiles, FtpFiles>()
                .AddTransient<IAsyncFtpClient, AsyncFtpClient>()
                .Configure<FtpFilesOptions>(builder.Configuration.GetSection("FtpFilesOptions"));
        }
    }
}
