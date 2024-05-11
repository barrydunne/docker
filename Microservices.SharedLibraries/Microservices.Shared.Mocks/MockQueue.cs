using Microservices.Shared.Queues;
using System.Collections.Concurrent;

namespace Microservices.Shared.Mocks;

/// <summary>
/// A basic mock queue that maintains all messages for verification purposes.
/// Messages will not be deleted regardless of the handled response from the consumer.
/// Transient subscription type is ignored.
/// </summary>
/// <typeparam name="TMessage">The type of message for this queue.</typeparam>
public class MockQueue<TMessage> : IQueue<TMessage>
{
    public ConcurrentBag<TMessage> Messages { get; }

    private readonly List<Func<TMessage, Task<bool>>> _consumers;
    private Exception? _publishException;

    public MockQueue()
    {
        Messages = new();
        _consumers = new();
    }

    public void WithPublishException(Exception? exception = null) => _publishException = exception ?? new InvalidOperationException();

    public Task PublishAsync(TMessage message, CancellationToken cancellationToken = default)
        => PostMessage(message);
    public Task SendAsync(TMessage message, CancellationToken cancellationToken = default)
        => PostMessage(message);
    public void StartReceiving(Func<TMessage, Task<bool>> handler)
        => _consumers.Add(handler);
    public void StartSubscribing(bool transientSubscription, Func<TMessage, Task<bool>> handler)
        => _consumers.Add(handler);
    public void Stop() { }

    private Task PostMessage(TMessage message)
    {
        if (_publishException is not null)
            throw _publishException;

        Messages.Add(message);
        foreach (var consumer in _consumers)
            consumer.Invoke(message);
        return Task.CompletedTask;
    }
}
