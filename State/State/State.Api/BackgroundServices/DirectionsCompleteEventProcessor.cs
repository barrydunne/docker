using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Mapster;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using State.Application.Commands.UpdateDirectionsResult;

namespace State.Api.BackgroundServices;

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
    /// <param name="traceActivity">The trace activity source.</param>
    /// <param name="logger">The logger to write to.</param>
    public DirectionsCompleteEventProcessor(IQueue<DirectionsCompleteEvent> queue, IServiceProvider serviceProvider, ITraceActivity traceActivity, ILogger<DirectionsCompleteEventProcessor> logger) : base(queue, serviceProvider, traceActivity, logger) { }

    /// <inheritdoc/>
    protected override UpdateDirectionsResultCommand CreateCommand(DirectionsCompleteEvent message) => message.Adapt<UpdateDirectionsResultCommand>();
}
