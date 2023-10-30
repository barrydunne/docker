using Microservices.Shared.Utilities;
using State.Api.HttpHandlers;
using State.Logic.Commands;
using State.Repository;

namespace State.Api
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
                .AddTransient<StateHandler>();

            // Repositories
            builder.Services
                .AddSingleton<IJobRepository, JobRepository>();

            // CQRS
            builder.Services
                .AddMediatR(_ => _.RegisterServicesFromAssembly(typeof(NotifyJobStatusUpdateCommand).Assembly));
        }
    }
}
