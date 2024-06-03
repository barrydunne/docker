using Email.Application.Commands.SendEmail;
using Email.Application.Queries.GetEmailsSentBetweenTimes;
using Email.Application.Queries.GetEmailsSentToRecipient;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Email.Infrastructure.Metrics;

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
            .AddSingleton<ISendEmailCommandHandlerMetrics, SendEmailCommandHandlerMetrics>()
            .AddSingleton<IGetEmailsSentToRecipientQueryHandlerMetrics, GetEmailsSentToRecipientQueryHandlerMetrics>()
            .AddSingleton<IGetEmailsSentBetweenTimesQueryHandlerMetrics, GetEmailsSentBetweenTimesQueryHandlerMetrics>();
    }
}
