using CSharpFunctionalExtensions;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using PublicApi.Logic.Commands;

namespace PublicApi.Api.BackgroundServices
{
    /// <summary>
    /// Processes <see cref="JobStatusUpdateEvent"/> messages.
    /// </summary>
    internal class JobStatusUpdateEventProcessor : QueueToCommandProcessor<JobStatusUpdateEvent, UpdateStatusCommand, Result>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobStatusUpdateEventProcessor"/> class.
        /// </summary>
        /// <param name="queue">The queue being processed.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
        /// <param name="logger">The logger to write to.</param>
        public JobStatusUpdateEventProcessor(IQueue<JobStatusUpdateEvent> queue, IServiceProvider serviceProvider, ILogger<JobStatusUpdateEventProcessor> logger) : base(queue, serviceProvider, logger) { }

        /// <inheritdoc/>
        protected override UpdateStatusCommand CreateCommand(JobStatusUpdateEvent message) => new(message.JobId, message.Status, message.Details);
    }
}
