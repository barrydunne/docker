using Microsoft.Extensions.DependencyInjection;
using PublicApi.Application.Commands.CreateJob;
using PublicApi.Application.Commands.UpdateStatus;
using PublicApi.Application.Queries.GetJobStatus;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace PublicApi.Infrastructure.Metrics;

/// <summary>
/// The shared metrics Meter container.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ApplicationMetrics
{
    static ApplicationMetrics() => Meter = new("PublicApi.Application");

    /// <summary>
    /// Gets the shared metrics Meter.
    /// </summary>
    internal static Meter Meter { get; }

    /// <summary>
    /// Register the required metrics providers.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <returns>The services instance.</returns>
    public static IServiceCollection RegisterMetrics(this IServiceCollection services)
    {
        return services
            .AddTransient<ICreateJobCommandHandlerMetrics, CreateJobCommandHandlerMetrics>()
            .AddTransient<IGetJobStatusQueryHandlerMetrics, GetJobStatusQueryHandlerMetrics>()
            .AddTransient<IUpdateStatusCommandHandlerMetrics, UpdateStatusCommandHandlerMetrics>();
    }
}
