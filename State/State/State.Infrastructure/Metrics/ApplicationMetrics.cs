using Microsoft.Extensions.DependencyInjection;
using State.Application.Commands.CreateJob;
using State.Application.Commands.NotifyJobStatusUpdate;
using State.Application.Commands.NotifyProcessingComplete;
using State.Application.Commands.UpdateDirectionsResult;
using State.Application.Commands.UpdateGeocodingResult;
using State.Application.Commands.UpdateImagingResult;
using State.Application.Commands.UpdateWeatherResult;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace State.Infrastructure.Metrics;

/// <summary>
/// The shared metrics Meter container.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ApplicationMetrics
{
    static ApplicationMetrics() => Meter = new("State.Application");

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
            .AddTransient<INotifyJobStatusUpdateCommandHandlerMetrics, NotifyJobStatusUpdateCommandHandlerMetrics>()
            .AddTransient<IUpdateGeocodingResultCommandHandlerMetrics, UpdateGeocodingResultCommandHandlerMetrics>()
            .AddTransient<IUpdateWeatherResultCommandHandlerMetrics, UpdateWeatherResultCommandHandlerMetrics>()
            .AddTransient<IUpdateDirectionsResultCommandHandlerMetrics, UpdateDirectionsResultCommandHandlerMetrics>()
            .AddTransient<IUpdateImagingResultCommandHandlerMetrics, UpdateImagingResultCommandHandlerMetrics>()
            .AddTransient<INotifyProcessingCompleteCommandHandlerMetrics, NotifyProcessingCompleteCommandHandlerMetrics>();
    }
}
