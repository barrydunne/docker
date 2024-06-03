using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Email.Application.Commands.SendEmail;
using Mapster;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;

namespace Email.Api.BackgroundServices;

/// <summary>
/// Processes <see cref="ProcessingCompleteEvent"/> messages.
/// </summary>
internal class ProcessingCompleteEventProcessor : QueueToCommandProcessor<ProcessingCompleteEvent, SendEmailCommand, Result>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessingCompleteEventProcessor"/> class.
    /// </summary>
    /// <param name="queue">The queue being processed.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
    /// <param name="traceActivity">The trace activity source.</param>
    /// <param name="logger">The logger to write to.</param>
    public ProcessingCompleteEventProcessor(IQueue<ProcessingCompleteEvent> queue, IServiceProvider serviceProvider, ITraceActivity traceActivity, ILogger<ProcessingCompleteEventProcessor> logger) : base(queue, serviceProvider, traceActivity, logger) { }

    /// <inheritdoc/>
    protected override SendEmailCommand CreateCommand(ProcessingCompleteEvent message) => message.Adapt<SendEmailCommand>();
}
