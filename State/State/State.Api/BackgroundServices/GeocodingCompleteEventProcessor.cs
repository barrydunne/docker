using AspNet.KickStarter.CQRS;
using Mapster;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using State.Application.Commands.UpdateGeocodingResult;

namespace State.Api.BackgroundServices;

/// <summary>
/// Processes <see cref="GeocodingCompleteEvent"/> messages.
/// </summary>
internal class GeocodingCompleteEventProcessor : QueueToCommandProcessor<GeocodingCompleteEvent, UpdateGeocodingResultCommand, Result>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeocodingCompleteEventProcessor"/> class.
    /// </summary>
    /// <param name="queue">The queue being processed.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
    /// <param name="logger">The logger to write to.</param>
    public GeocodingCompleteEventProcessor(IQueue<GeocodingCompleteEvent> queue, IServiceProvider serviceProvider, ILogger<GeocodingCompleteEventProcessor> logger) : base(queue, serviceProvider, logger) { }

    /// <inheritdoc/>
    protected override UpdateGeocodingResultCommand CreateCommand(GeocodingCompleteEvent message) => message.Adapt<UpdateGeocodingResultCommand>();
}
