using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Directions.Application.Commands.GenerateDirections;
using Mapster;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;

namespace Directions.Api.BackgroundServices;

/// <summary>
/// Processes <see cref="LocationsReadyEvent"/> messages.
/// </summary>
internal class LocationsReadyEventProcessor : QueueToCommandProcessor<LocationsReadyEvent, GenerateDirectionsCommand, Result>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocationsReadyEventProcessor"/> class.
    /// </summary>
    /// <param name="queue">The queue being processed.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
    /// <param name="traceActivity">The trace activity source.</param>
    /// <param name="logger">The logger to write to.</param>
    public LocationsReadyEventProcessor(IQueue<LocationsReadyEvent> queue, IServiceProvider serviceProvider, ITraceActivity traceActivity, ILogger<LocationsReadyEventProcessor> logger) : base(queue, serviceProvider, traceActivity, logger) { }

    /// <inheritdoc/>
    protected override GenerateDirectionsCommand CreateCommand(LocationsReadyEvent message) => message.Adapt<GenerateDirectionsCommand>();
}
