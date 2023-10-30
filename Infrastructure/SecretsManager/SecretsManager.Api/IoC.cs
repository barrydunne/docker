using AspNet.KickStarter.HttpHandlers;
using SecretsManager.Api.HttpHandlers;
using SecretsManager.Logic.Queries;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.IO.Abstractions;

namespace SecretsManager.Api
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
        /// <exception cref="KeyNotFoundException">If the Redis configuration is not found.</exception>
        internal static void RegisterServices(WebApplicationBuilder builder)
        {
            // API Handlers
            builder.Services
                .AddTransient<HealthHandler>()
                .AddTransient<SecretsHandler>();

            // FileSystem (Needed by HealthHandler)
            builder.Services
                .AddSingleton<IFileSystem, FileSystem>();

            // CQRS
            builder.Services
                .AddMediatR(_ => _.RegisterServicesFromAssembly(typeof(GetSecretsQuery).Assembly));

            // Redis
            var conf = builder.Configuration.GetSection("Redis").Get<RedisConfiguration>() ?? throw new KeyNotFoundException("Redis configuration not found");
            builder.Services
                .AddStackExchangeRedisExtensions<NewtonsoftSerializer>(conf);
        }
    }
}
