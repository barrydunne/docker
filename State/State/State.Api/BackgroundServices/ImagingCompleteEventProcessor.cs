using AspNet.KickStarter.CQRS;
using Mapster;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using State.Application.Commands.UpdateImagingResult;

namespace State.Api.BackgroundServices;

/// <summary>
/// Processes <see cref="ImagingCompleteEvent"/> messages.
/// </summary>
internal class ImagingCompleteEventProcessor : QueueToCommandProcessor<ImagingCompleteEvent, UpdateImagingResultCommand, Result>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImagingCompleteEventProcessor"/> class.
    /// </summary>
    /// <param name="queue">The queue being processed.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
    /// <param name="logger">The logger to write to.</param>
    public ImagingCompleteEventProcessor(IQueue<ImagingCompleteEvent> queue, IServiceProvider serviceProvider, ILogger<ImagingCompleteEventProcessor> logger) : base(queue, serviceProvider, logger) { }

    /// <inheritdoc/>
    protected override UpdateImagingResultCommand CreateCommand(ImagingCompleteEvent message) => message.Adapt<UpdateImagingResultCommand>();
}
