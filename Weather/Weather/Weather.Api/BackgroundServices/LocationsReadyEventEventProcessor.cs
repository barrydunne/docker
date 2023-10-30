using CSharpFunctionalExtensions;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using Weather.Logic.Commands;

namespace Weather.Api.BackgroundServices
{
    /// <summary>
    /// Processes <see cref="LocationsReadyEvent"/> messages.
    /// </summary>
    internal class LocationsReadyEventProcessor : QueueToCommandProcessor<LocationsReadyEvent, GenerateWeatherCommand, Result>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationsReadyEventProcessor"/> class.
        /// </summary>
        /// <param name="queue">The queue being processed.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
        /// <param name="logger">The logger to write to.</param>
        public LocationsReadyEventProcessor(IQueue<LocationsReadyEvent> queue, IServiceProvider serviceProvider, ILogger<LocationsReadyEventProcessor> logger) : base(queue, serviceProvider, logger) { }

        /// <inheritdoc/>
        protected override GenerateWeatherCommand CreateCommand(LocationsReadyEvent message) => new(message.JobId, message.DestinationCoordinates);
    }
}
