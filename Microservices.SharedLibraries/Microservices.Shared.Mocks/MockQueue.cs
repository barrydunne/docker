using Microservices.Shared.Queues;
using Moq;
using System.Collections.Concurrent;

namespace Microservices.Shared.Mocks;

/// <summary>
/// A basic mock queue that maintains all messages for verification purposes.
/// Messages will not be deleted regardless of the handled response from the consumer.
/// Transient subscription type is ignored.
/// </summary>
/// <typeparam name="TMessage">The type of message for this queue.</typeparam>
public class MockQueue<TMessage> : Mock<IQueue<TMessage>>
{
    public ConcurrentBag<TMessage> Messages { get; }

    private readonly List<Func<TMessage, Task<bool>>> _consumers;

    public MockQueue() : base(MockBehavior.Strict)
    {
        Messages = new();
        _consumers = new();

        Setup(_ => _.SendAsync(It.IsAny<TMessage>(), It.IsAny<CancellationToken>()))
            .Callback((TMessage message, CancellationToken _) => PostMessage(message))
            .Returns(Task.CompletedTask);

        Setup(_ => _.PublishAsync(It.IsAny<TMessage>(), It.IsAny<CancellationToken>()))
            .Callback((TMessage message, CancellationToken _) => PostMessage(message))
            .Returns(Task.CompletedTask);

        Setup(_ => _.StartReceiving(It.IsAny<Func<TMessage, Task<bool>>>()))
            .Callback((Func<TMessage, Task<bool>> handler) => _consumers.Add(handler));

        Setup(_ => _.StartSubscribing(It.IsAny<bool>(), It.IsAny<Func<TMessage, Task<bool>>>()))
            .Callback((bool _, Func<TMessage, Task<bool>> handler) => _consumers.Add(handler));
    }

    private void PostMessage(TMessage message)
    {
        Messages.Add(message);
        foreach (var consumer in  _consumers)
            consumer.Invoke(message);
    }
}
