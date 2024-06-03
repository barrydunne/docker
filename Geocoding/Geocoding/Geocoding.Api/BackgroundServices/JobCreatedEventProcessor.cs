using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Geocoding.Application.Commands.GeocodeAddresses;
using Mapster;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;

namespace Geocoding.Api.BackgroundServices;

/// <summary>
/// Processes <see cref="JobCreatedEvent"/> messages.
/// </summary>
internal class JobCreatedEventProcessor : QueueToCommandProcessor<JobCreatedEvent, GeocodeAddressesCommand, Result>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JobCreatedEventProcessor"/> class.
    /// </summary>
    /// <param name="queue">The queue being processed.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
    /// <param name="traceActivity">The trace activity source.</param>
    /// <param name="logger">The logger to write to.</param>
    public JobCreatedEventProcessor(IQueue<JobCreatedEvent> queue, IServiceProvider serviceProvider, ITraceActivity traceActivity, ILogger<JobCreatedEventProcessor> logger) : base(queue, serviceProvider, traceActivity, logger) { }

    /// <inheritdoc/>
    protected override GeocodeAddressesCommand CreateCommand(JobCreatedEvent message) => message.Adapt<GeocodeAddressesCommand>();
}
