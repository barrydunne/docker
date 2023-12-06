namespace Microservices.Shared.Queues;

/// <summary>
/// Provides interaction with a message queue.
/// </summary>
/// <typeparam name="TMessage">The type of message for this queue.</typeparam>
public interface IQueue<TMessage>
{
    /// <summary>
    /// Send a message to a queue.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendAsync(TMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish a message to subscribers.
    /// </summary>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task PublishAsync(TMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Start receiving messages from a queue.
    /// </summary>
    /// <param name="handler">The handler for the messages, returns true if the message was processed and may be deleted.</param>
    void StartReceiving(Func<TMessage, Task<bool>> handler);

    /// <summary>
    /// Start subscribing to messages.
    /// </summary>
    /// <param name="transientSubscription">Whether to use a temporary subscription that will only receive messages while running.</param>
    /// <param name="handler">The handler for the messages, returns true if the message was processed and may be deleted.</param>
    void StartSubscribing(bool transientSubscription, Func<TMessage, Task<bool>> handler);

    /// <summary>
    /// Stop receiving messages.
    /// </summary>
    void Stop();
}
