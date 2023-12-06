using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Concurrent;

namespace Microservices.Shared.Mocks;

public class MockLogger<T> : Mock<ILogger<T>>
{
    private readonly ConcurrentQueue<string> _log;

    public IReadOnlyCollection<string> Log => _log.ToArray();

    public MockLogger(MockBehavior behavior = MockBehavior.Strict) : base(behavior)
    {
        _log = new();

        Setup(_ => _.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(_ =>
            {
                var logLevel = (LogLevel)_.Arguments[0];
                var eventId = (EventId)_.Arguments[1];
                var state = _.Arguments[2];
                var exception = (Exception?)_.Arguments[3];
                var formatter = _.Arguments[4];
                var invokeMethod = formatter.GetType().GetMethod("Invoke");
                var message = (string?)invokeMethod?.Invoke(formatter, new[] { state, exception });
                Write((LogLevel)_.Arguments[0], (Exception?)_.Arguments[3], (string?)invokeMethod?.Invoke(formatter, new[] { state, exception }) ?? string.Empty);
            }));
    }

    private void Write(LogLevel logLevel, Exception? exception, string message)
        => _log.Enqueue($"[{logLevel}] {message}{(exception is null ? "" : $" ({exception?.Message}")}");
}
