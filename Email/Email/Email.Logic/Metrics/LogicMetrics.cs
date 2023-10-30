using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Email.Logic.Metrics
{
    /// <summary>
    /// The shared metrics Meter container.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class LogicMetrics
    {
        static LogicMetrics() => Meter = new("Email.Logic");

        /// <summary>
        /// Gets the shared metrics Meter.
        /// </summary>
        internal static Meter Meter { get; }

        /// <summary>
        /// Register the required metrics providers.
        /// </summary>
        /// <param name="services">The service collection to register with.</param>
        /// <returns>The services instance.</returns>
        public static IServiceCollection RegisterLogicMetrics(this IServiceCollection services)
        {
            return services
                .AddTransient<ISendEmailCommandHandlerMetrics, SendEmailCommandHandlerMetrics>()
                .AddTransient<IGetEmailsSentToRecipientQueryHandlerMetrics, GetEmailsSentToRecipientQueryHandlerMetrics>()
                .AddTransient<IGetEmailsSentBetweenTimesQueryHandlerMetrics, GetEmailsSentBetweenTimesQueryHandlerMetrics>();
        }
    }
}
