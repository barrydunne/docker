using Microsoft.Extensions.Logging;

namespace Microservices.Shared.Queues;

/// <summary>
/// The base class for processing messages from a queue.
/// </summary>
/// <typeparam name="TMessage">The type of message for this queue.</typeparam>
public abstract class BaseQueueProcessor<TMessage> : IDisposable
{
    /// <summary>
    /// The queue being processed.
    /// </summary>
    protected readonly IQueue<TMessage> _queue;

    /// <summary>
    /// The logger to write to.
    /// </summary>
    protected readonly ILogger _logger;

    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseQueueProcessor{T}"/> class.
    /// </summary>
    /// <param name="queue">The queue being processed.</param>
    /// <param name="logger">The logger to write to.</param>
    protected BaseQueueProcessor(IQueue<TMessage> queue, ILogger logger)
    {
        _queue = queue;
        _logger = logger;
    }

    /// <summary>
    /// Start to receive messages sent to the single queue.
    /// </summary>
    public void StartReceiving()
    {
        try
        {
            _logger.LogInformation("Starting {Type} queue handling", typeof(TMessage).Name);
            _queue.StartReceiving(ProcessMessageAsync);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start handling {Type} queue", typeof(TMessage).Name);
        }
    }

    /// <summary>
    /// Start to receive messages published to an exchange.
    /// </summary>
    /// <param name="transientSubscription">
    /// A transient subscription will create a temporary queue for the lifetime of this process.
    /// When true, every running instance of the application will receive messages that are published while it is running.
    /// When false, a single durable queue will be bound to the exchange and only one instance of the application will get each message.
    /// </param>
    public void StartSubscribing(bool transientSubscription = true)
    {
        try
        {
            _logger.LogInformation("Starting {Type} subscription", typeof(TMessage).Name);
            _queue.StartSubscribing(transientSubscription, ProcessMessageAsync);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start subscription for {Type}", typeof(TMessage).Name);
        }
    }

    /// <summary>
    /// The handler for the messages, returns true if the message was processed and may be deleted.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    /// <returns>True if the message was processed and may be deleted.</returns>
    protected abstract Task<bool> ProcessMessageAsync(TMessage message);

    /// <summary>
    /// Dispose of this processor.
    /// </summary>
    /// <param name="disposing">Whether to dispose of resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                (_queue as IDisposable)?.Dispose();
            _disposedValue = true;
        }
    }

    /// <summary>
    /// Dispose of this processor.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
