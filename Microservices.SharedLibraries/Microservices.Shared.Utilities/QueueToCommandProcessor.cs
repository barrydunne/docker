using AspNet.KickStarter.CQRS;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microservices.Shared.Utilities;

/// <summary>
/// Receives messages and dispatches commands.
/// </summary>
/// <typeparam name="TMessage">The type of message for this queue.</typeparam>
/// <typeparam name="TCommand">The type of command to dispatch.</typeparam>
/// <typeparam name="TResult">The type of command result.</typeparam>
public abstract class QueueToCommandProcessor<TMessage, TCommand, TResult> : BaseQueueProcessor<TMessage>
    where TMessage : BaseEvent
    where TCommand : IRequest<TResult>
    where TResult : IResult
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueToCommandProcessor{TMessage, TCommand, TResult}"/> class.
    /// </summary>
    /// <param name="queue">The queue being processed.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
    /// <param name="logger">The logger to write to.</param>
    protected QueueToCommandProcessor(IQueue<TMessage> queue, IServiceProvider serviceProvider, ILogger logger) : base(queue, logger) => _serviceProvider = serviceProvider;

    /// <inheritdoc/>
    protected override async Task<bool> ProcessMessageAsync(TMessage message)
    {
        _logger.LogInformation("New {MessageType} message received. [{CorrelationID}]", typeof(TMessage).Name, message.JobId);
        try
        {
            // Note: In this simple application no checks will be performed to ensure that a newer status update message has not already been handled.
            var command = CreateCommand(message);

            // Use a custom scope so that all resolved service lifetimes are limited to the duration of message processing.
            // Otherwise IDisposable types would only be disposed when the application is shutting down.
            // That would cause connections to RabbitMQ to remain open and exhaust available ports.
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
            var result = await mediator.Send(command);
            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process {MessageType} message. [{CorrelationID}]", typeof(TMessage).Name, message.JobId);
            return false;
        }
    }

    /// <summary>
    /// Create a command from the message.
    /// </summary>
    /// <param name="message">The message being processed.</param>
    /// <returns>The command to dispatch.</returns>
    protected abstract TCommand CreateCommand(TMessage message);
}
