using Microsoft.Extensions.DependencyInjection;
using State.Application.Commands.CreateJob;
using State.Application.Commands.NotifyJobStatusUpdate;
using State.Application.Commands.NotifyProcessingComplete;
using State.Application.Commands.UpdateDirectionsResult;
using State.Application.Commands.UpdateGeocodingResult;
using State.Application.Commands.UpdateImagingResult;
using State.Application.Commands.UpdateWeatherResult;
using System.Diagnostics.CodeAnalysis;

namespace State.Infrastructure.Metrics;

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
            .AddSingleton<INotifyJobStatusUpdateCommandHandlerMetrics, NotifyJobStatusUpdateCommandHandlerMetrics>()
            .AddSingleton<IUpdateGeocodingResultCommandHandlerMetrics, UpdateGeocodingResultCommandHandlerMetrics>()
            .AddSingleton<IUpdateWeatherResultCommandHandlerMetrics, UpdateWeatherResultCommandHandlerMetrics>()
            .AddSingleton<IUpdateDirectionsResultCommandHandlerMetrics, UpdateDirectionsResultCommandHandlerMetrics>()
            .AddSingleton<IUpdateImagingResultCommandHandlerMetrics, UpdateImagingResultCommandHandlerMetrics>()
            .AddSingleton<INotifyProcessingCompleteCommandHandlerMetrics, NotifyProcessingCompleteCommandHandlerMetrics>();
    }
}
