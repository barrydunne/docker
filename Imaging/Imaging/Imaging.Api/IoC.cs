using FluentFTP;
using Imaging.Api.HttpHandlers;
using Imaging.ExternalService;
using Imaging.Logic.Commands;
using Microservices.Shared.CloudFiles;
using Microservices.Shared.CloudFiles.Ftp;
using Microservices.Shared.Utilities;

namespace Imaging.Api
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
                .AddTransient<ImagingHandler>();

            // External service
            builder.Services
                .AddTransient<IExternalService, Dummy>(); // Replace Dummy with Bing for real images

            // CQRS
            builder.Services
                .AddMediatR(_ => _.RegisterServicesFromAssembly(typeof(SaveImageCommand).Assembly));

            // Cloud Services
            builder.Services
                .AddTransient<ICloudFiles, FtpFiles>()
                .AddTransient<IAsyncFtpClient, AsyncFtpClient>()
                .Configure<FtpFilesOptions>(builder.Configuration.GetSection("FtpFilesOptions"));
        }
    }
}
