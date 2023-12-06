using AspNet.KickStarter.CQRS;
using MediatR;
using Microservices.Shared.Events;
using Microsoft.Extensions.Hosting;

namespace Microservices.Shared.Utilities;

/// <summary>
/// The <see cref="BackgroundService"/> that triggers the <see cref="QueueBackgroundService{TMessage, TCommand, TResult}"/> class to subscribe to events.
/// </summary>
/// <typeparam name="TMessage">The type of message for this queue.</typeparam>
/// <typeparam name="TCommand">The type of command to dispatch.</typeparam>
/// <typeparam name="TResult">The type of command result.</typeparam>
public class QueueBackgroundService<TMessage, TCommand, TResult> : BackgroundService
    where TMessage : BaseEvent
    where TCommand : IRequest<TResult>
    where TResult : IResult
{
    private readonly QueueToCommandProcessor<TMessage, TCommand, TResult> _processor;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueBackgroundService{TMessage, TCommand, TResult}"/> class.
    /// </summary>
    /// <param name="processor">The event processor.</param>
    public QueueBackgroundService(QueueToCommandProcessor<TMessage, TCommand, TResult> processor) => _processor = processor;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield(); // https://github.com/dotnet/runtime/issues/36063
        _processor.StartSubscribing(transientSubscription: false);
    }
}
