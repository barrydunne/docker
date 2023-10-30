using CSharpFunctionalExtensions;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using State.Logic.Commands;

namespace State.Api.BackgroundServices
{
    /// <summary>
    /// Processes <see cref="JobCreatedEvent"/> messages.
    /// </summary>
    internal class JobCreatedEventProcessor : QueueToCommandProcessor<JobCreatedEvent, CreateJobCommand, Result>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobCreatedEventProcessor"/> class.
        /// </summary>
        /// <param name="queue">The queue being processed.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
        /// <param name="logger">The logger to write to.</param>
        public JobCreatedEventProcessor(IQueue<JobCreatedEvent> queue, IServiceProvider serviceProvider, ILogger<JobCreatedEventProcessor> logger) : base(queue, serviceProvider, logger) { }

        /// <inheritdoc/>
        protected override CreateJobCommand CreateCommand(JobCreatedEvent message) => new(message.JobId, message.StartingAddress, message.DestinationAddress, message.Email);
    }
}
