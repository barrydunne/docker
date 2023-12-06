using Email.Application.Commands.SendEmail;
using Email.Application.Queries.GetEmailsSentBetweenTimes;
using Email.Application.Queries.GetEmailsSentToRecipient;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Email.Infrastructure.Metrics;

/// <summary>
/// The shared metrics Meter container.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ApplicationMetrics
{
    static ApplicationMetrics() => Meter = new("Email.Application");

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
            .AddTransient<ISendEmailCommandHandlerMetrics, SendEmailCommandHandlerMetrics>()
            .AddTransient<IGetEmailsSentToRecipientQueryHandlerMetrics, GetEmailsSentToRecipientQueryHandlerMetrics>()
            .AddTransient<IGetEmailsSentBetweenTimesQueryHandlerMetrics, GetEmailsSentBetweenTimesQueryHandlerMetrics>();
    }
}
