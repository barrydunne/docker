using Microsoft.Extensions.DependencyInjection;
using PublicApi.Application.Commands.CreateJob;
using PublicApi.Application.Commands.UpdateStatus;
using PublicApi.Application.Queries.GetJobStatus;
using System.Diagnostics.CodeAnalysis;

namespace PublicApi.Infrastructure.Metrics;

/// <summary>
/// The shared metrics Meter container.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ApplicationMetrics
{
    /// <summary>
    /// Register the required metrics providers.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <returns>The services instance.</returns>
    public static IServiceCollection RegisterMetrics(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICreateJobCommandHandlerMetrics, CreateJobCommandHandlerMetrics>()
            .AddSingleton<IGetJobStatusQueryHandlerMetrics, GetJobStatusQueryHandlerMetrics>()
            .AddSingleton<IUpdateStatusCommandHandlerMetrics, UpdateStatusCommandHandlerMetrics>();
    }
}
