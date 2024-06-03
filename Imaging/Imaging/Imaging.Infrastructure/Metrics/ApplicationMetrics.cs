using Imaging.Application.Commands.SaveImage;
using Imaging.Application.Queries.GetImageUrl;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Imaging.Infrastructure.Metrics;

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
            .AddSingleton<ISaveImageCommandHandlerMetrics, SaveImageCommandHandlerMetrics>()
            .AddSingleton<IGetImageUrlQueryHandlerMetrics, GetImageUrlQueryHandlerMetrics>();
    }
}
