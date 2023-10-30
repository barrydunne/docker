using Geocoding.Api.HttpHandlers;
using Geocoding.ExternalService;
using Geocoding.Logic.Caching;
using Geocoding.Logic.Commands;
using Microservices.Shared.Utilities;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace Geocoding.Api
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
                .AddTransient<GeocodingHandler>();

            // External service
            builder.Services
                .AddTransient<IExternalService, Dummy>(); // Replace Dummy with MapQuest for real geocoding

            // CQRS
            builder.Services
                .AddMediatR(_ => _.RegisterServicesFromAssembly(typeof(GeocodeAddressesCommand).Assembly));

            // Caching
            var conf = builder.Configuration.GetSection("Redis").Get<RedisConfiguration>() ?? throw new KeyNotFoundException("Redis configuration not found");
            builder.Services
                .AddSingleton<IGeocodingCache, GeocodingCache>()
                .AddStackExchangeRedisExtensions<NewtonsoftSerializer>(conf);
        }
    }
}
