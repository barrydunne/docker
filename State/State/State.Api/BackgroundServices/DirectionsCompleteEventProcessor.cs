using CSharpFunctionalExtensions;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using State.Logic.Commands;

namespace State.Api.BackgroundServices
{
    /// <summary>
    /// Processes <see cref="DirectionsCompleteEvent"/> messages.
    /// </summary>
    internal class DirectionsCompleteEventProcessor : QueueToCommandProcessor<DirectionsCompleteEvent, UpdateDirectionsResultCommand, Result>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectionsCompleteEventProcessor"/> class.
        /// </summary>
        /// <param name="queue">The queue being processed.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
        /// <param name="logger">The logger to write to.</param>
        public DirectionsCompleteEventProcessor(IQueue<DirectionsCompleteEvent> queue, IServiceProvider serviceProvider, ILogger<DirectionsCompleteEventProcessor> logger) : base(queue, serviceProvider, logger) { }

        /// <inheritdoc/>
        protected override UpdateDirectionsResultCommand CreateCommand(DirectionsCompleteEvent message) => new(message.JobId, message.Directions);
    }
}
