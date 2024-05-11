using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Microservices.Shared.Mocks;

public class MockLogger<T> : ILogger<T>
{
    private readonly ConcurrentQueue<string> _log;

    public IReadOnlyCollection<string> Messages => _log.ToArray();

    public MockLogger() => _log = new();

    private void Write(LogLevel logLevel, Exception? exception, string message)
    {
        var logMessage = $"[{logLevel}] {message}{(exception is null ? "" : $" ({exception?.Message}")}";
        Trace.WriteLine(logMessage);
        _log.Enqueue(logMessage);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        => Write(logLevel, exception, (string?)formatter.GetType().GetMethod("Invoke")?.Invoke(formatter, [state, exception]) ?? string.Empty);

    public bool IsEnabled(LogLevel logLevel) => true;
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}
