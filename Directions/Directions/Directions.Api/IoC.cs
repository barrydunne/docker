using Directions.Api.HttpHandlers;
using Directions.ExternalService;
using Directions.Logic.Commands;
using Microservices.Shared.Utilities;

namespace Directions.Api
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
                .AddTransient<DirectionsHandler>();

            // External service
            builder.Services
                .AddTransient<IExternalService, Dummy>(); // Replace Dummy with MapQuest for real directions

            // CQRS
            builder.Services
                .AddMediatR(_ => _.RegisterServicesFromAssembly(typeof(GenerateDirectionsCommand).Assembly));
        }
    }
}
