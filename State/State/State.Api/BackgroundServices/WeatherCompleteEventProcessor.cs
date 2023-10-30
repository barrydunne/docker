using CSharpFunctionalExtensions;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using State.Logic.Commands;

namespace State.Api.BackgroundServices
{
    /// <summary>
    /// Processes <see cref="WeatherCompleteEvent"/> messages.
    /// </summary>
    internal class WeatherCompleteEventProcessor : QueueToCommandProcessor<WeatherCompleteEvent, UpdateWeatherResultCommand, Result>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherCompleteEventProcessor"/> class.
        /// </summary>
        /// <param name="queue">The queue being processed.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
        /// <param name="logger">The logger to write to.</param>
        public WeatherCompleteEventProcessor(IQueue<WeatherCompleteEvent> queue, IServiceProvider serviceProvider, ILogger<WeatherCompleteEventProcessor> logger) : base(queue, serviceProvider, logger) { }

        /// <inheritdoc/>
        protected override UpdateWeatherResultCommand CreateCommand(WeatherCompleteEvent message) => new(message.JobId, message.Weather);
    }
}
