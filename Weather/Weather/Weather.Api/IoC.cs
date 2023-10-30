using Microservices.Shared.Utilities;
using Weather.Api.HttpHandlers;
using Weather.ExternalService;
using Weather.Logic.Commands;

namespace Weather.Api
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
                .AddTransient<WeatherHandler>();

            // External service
            builder.Services
                .AddTransient<IExternalService, Dummy>(); // Replace Dummy with OpenMeteo for real weather

            // CQRS
            builder.Services
                .AddMediatR(_ => _.RegisterServicesFromAssembly(typeof(GenerateWeatherCommand).Assembly));
        }
    }
}
