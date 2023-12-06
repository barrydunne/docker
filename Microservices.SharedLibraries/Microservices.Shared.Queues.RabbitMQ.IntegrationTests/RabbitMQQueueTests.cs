using Microservices.Shared.Queues.RabbitMQ.IntegrationTests.QueueModels;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace Microservices.Shared.Queues.RabbitMQ.IntegrationTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[NonParallelizable]
[TestFixture(Category = "RabbitMQ")]
public class RabbitMQQueueTests : IDisposable
{
    private readonly Fixture _fixture = new();
    private readonly RabbitMQQueueTestsContext _context = new();

    private bool _disposedValue;

    [Test]
    public async Task SendAsync_sends_to_a_durable_permanent_queue()
    {
        using var sut = _context.Sut<SendMessage>();
        await sut.SendAsync(_fixture.Create<SendMessage>());
        await _context.AssertQueueExistsAsync(nameof(SendMessage), durable: true, autoDelete: false);
    }

    [Test]
    public async Task PublishAsync_publishes_to_a_direct_exchange()
    {
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(_fixture.Create<PublishMessage>());
        await _context.AssertExchangeExistsAsync(nameof(PublishMessage), type: ExchangeType.Direct);
    }

    [Test]
    public async Task StartReceiving_reads_from_a_durable_permanent_queue()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartReceiving((_) => Task.FromResult(true));
        sut.Stop();
        await _context.AssertQueueExistsAsync(nameof(SendMessage), durable: true, autoDelete: false);
    }

    [Test]
    public async Task StartReceiving_receives_sent_messages()
    {
        var message = _fixture.Create<SendMessage>();
        var received = new ConcurrentBag<SendMessage>();
        using var sut = _context.Sut<SendMessage>();
        sut.StartReceiving((_) => 
        {
            received.Add(_);
            return Task.FromResult(true);
        });
        await sut.SendAsync(message);
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while ((received.Count == 0) && (DateTime.UtcNow < timeout))
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        sut.Stop();
        Assert.That(received, Does.Contain(message));
    }

    [Test]
    public async Task StartReceiving_deletes_message_if_handled()
    {
        var message = _fixture.Create<SendMessage>();
        var received = false;
        using var sut = _context.Sut<SendMessage>();
        sut.StartReceiving((_) =>
        {
            received = true;
            return Task.FromResult(true);
        });
        await sut.SendAsync(message);
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while ((!received) && (DateTime.UtcNow < timeout))
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        sut.Stop();
        await _context.AssertQueueMessageCount(nameof(SendMessage), 0);
    }

    [Test]
    public async Task StartReceiving_does_not_delete_message_if_not_handled()
    {
        var message = _fixture.Create<SendMessage>();
        var received = false;
        using var sut = _context.Sut<SendMessage>();
        sut.StartReceiving((_) =>
        {
            received = true;
            return Task.FromResult(false);
        });
        await sut.SendAsync(message);
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while ((!received) && (DateTime.UtcNow < timeout))
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        sut.Stop();
        await _context.AssertQueueMessageCount(nameof(SendMessage), 1);
    }

    [Test]
    public async Task StartSubscribing_transient_reads_from_a_non_durable_autodelete_queue()
    {
        var message = _fixture.Create<PublishMessage>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(true, (_) => Task.FromResult(true));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var queueName = await _context.GetQueueBoundToExchangeAsync(nameof(PublishMessage));
        await _context.AssertQueueExistsAsync(queueName!, durable: false, autoDelete: true);
    }

    [Test]
    public async Task StartSubscribing_transient_receives_sent_messages()
    {
        var message = _fixture.Create<PublishMessage>();
        var received = new ConcurrentBag<PublishMessage>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(true, (_) =>
        {
            received.Add(_);
            return Task.FromResult(true);
        });
        await sut.PublishAsync(message);
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while ((received.Count == 0) && (DateTime.UtcNow < timeout))
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        Assert.That(received, Does.Contain(message));
    }

    [Test]
    public async Task StartSubscribing_transient_deletes_message_if_handled()
    {
        var message = _fixture.Create<PublishMessage>();
        var received = false;
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(true, (_) =>
        {
            received = true;
            return Task.FromResult(true);
        });
        await sut.PublishAsync(message);
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while ((!received) && (DateTime.UtcNow < timeout))
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var queueName = await _context.GetQueueBoundToExchangeAsync(nameof(PublishMessage));
        await _context.AssertQueueMessageCount(queueName!, 0);
    }

    [Test]
    public async Task StartSubscribing_transient_does_not_delete_message_if_not_handled()
    {
        var message = _fixture.Create<PublishMessage>();
        var received = false;
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(true, (_) =>
        {
            received = true;
            Thread.Sleep(TimeSpan.FromSeconds(1));
            return Task.FromResult(false);
        });
        await sut.PublishAsync(message);
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while ((!received) && (DateTime.UtcNow < timeout))
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var queueName = await _context.GetQueueBoundToExchangeAsync(nameof(PublishMessage));
        await _context.AssertQueueMessageCount(queueName!, 1);
    }

    [Test]
    public async Task StartSubscribing_non_transient_reads_from_a_durable_permanent_queue()
    {
        var message = _fixture.Create<PublishMessage>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var queueName = $"{nameof(PublishMessage)}-{_context.Suffix}";
        await _context.AssertQueueExistsAsync(queueName!, durable: true, autoDelete: false);
    }

    [Test]
    public async Task StartSubscribing_non_transient_receives_sent_messages()
    {
        var message = _fixture.Create<PublishMessage>();
        var received = new ConcurrentBag<PublishMessage>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(false, (_) =>
        {
            received.Add(_);
            return Task.FromResult(true);
        });
        await sut.PublishAsync(message);
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while ((received.Count == 0) && (DateTime.UtcNow < timeout))
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        Assert.That(received, Does.Contain(message));
    }

    [Test]
    public async Task StartSubscribing_non_transient_deletes_message_if_handled()
    {
        var message = _fixture.Create<PublishMessage>();
        var received = false;
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(false, (_) =>
        {
            received = true;
            return Task.FromResult(true);
        });
        await sut.PublishAsync(message);
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while ((!received) && (DateTime.UtcNow < timeout))
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var queueName = $"{nameof(PublishMessage)}-{_context.Suffix}";
        await _context.AssertQueueMessageCount(queueName, 0);
    }

    [Test]
    public async Task StartSubscribing_non_transient_does_not_delete_message_if_not_handled()
    {
        var message = _fixture.Create<PublishMessage>();
        var received = false;
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(false, (_) =>
        {
            received = true;
            Thread.Sleep(TimeSpan.FromSeconds(1));
            return Task.FromResult(false);
        });
        await sut.PublishAsync(message);
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while ((!received) && (DateTime.UtcNow < timeout))
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var queueName = $"{nameof(PublishMessage)}-{_context.Suffix}";
        await _context.AssertQueueMessageCount(queueName, 1);
    }

    [Test]
    public async Task StartSubscribing_non_transient_with_retry_creates_queue()
    {
        _context.WithRetryDelayMilliseconds(100);
        var message = _fixture.Create<PublishMessage>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var queueName = $"{nameof(PublishMessage)}-{_context.Suffix}";
        await _context.AssertQueueExistsAsync(queueName!, durable: true, autoDelete: false);
    }

    [Test]
    public async Task StartSubscribing_non_transient_with_retry_creates_queue_bound_to_exchange_for_all_messages()
    {
        _context.WithRetryDelayMilliseconds(100);
        var message = _fixture.Create<PublishMessage>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var queueName = $"{nameof(PublishMessage)}-{_context.Suffix}";
        await _context.AssertBindingExistsAsync(queueName!, nameof(PublishMessage), "ALL");
    }

    [Test]
    public async Task StartSubscribing_non_transient_with_retry_creates_queue_bound_to_exchange_for_own_retry_messages()
    {
        _context.WithRetryDelayMilliseconds(100);
        var message = _fixture.Create<PublishMessage>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var queueName = $"{nameof(PublishMessage)}-{_context.Suffix}";
        await _context.AssertBindingExistsAsync(queueName!, nameof(PublishMessage), _context.Suffix);
    }

    [Test]
    public async Task StartSubscribing_non_transient_with_retry_creates_queue_with_dead_letter_exchange()
    {
        _context.WithRetryDelayMilliseconds(100);
        var message = _fixture.Create<PublishMessage>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var queueName = $"{nameof(PublishMessage)}-{_context.Suffix}";
        var exchangeName = $"{nameof(PublishMessage)}-{_context.Suffix}-retry";
        await _context.AssertDeadLetterExchangeExistsAsync(queueName!, exchangeName!);
    }

    [Test]
    public async Task StartSubscribing_non_transient_with_retry_creates_retry_exchange()
    {
        _context.WithRetryDelayMilliseconds(100);
        var message = _fixture.Create<PublishMessage>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var exchangeName = $"{nameof(PublishMessage)}-{_context.Suffix}-retry";
        await _context.AssertExchangeExistsAsync(exchangeName, type: ExchangeType.Direct);
    }

    [Test]
    public async Task StartSubscribing_non_transient_with_retry_creates_retry_queue()
    {
        _context.WithRetryDelayMilliseconds(100);
        var message = _fixture.Create<PublishMessage>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var queueName = $"{nameof(PublishMessage)}-{_context.Suffix}-retry";
        await _context.AssertQueueExistsAsync(queueName!, durable: true, autoDelete: false);
    }

    [Test]
    public async Task StartSubscribing_non_transient_with_retry_creates_retry_queue_bound_to_retry_exchange()
    {
        _context.WithRetryDelayMilliseconds(100);
        var message = _fixture.Create<PublishMessage>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        var retryName = $"{nameof(PublishMessage)}-{_context.Suffix}-retry";
        await _context.AssertBindingExistsAsync(retryName!, retryName, string.Empty);
    }

    [Test]
    public async Task StartSubscribing_non_transient_transient_receives_retried_messages()
    {
        var message = _fixture.Create<PublishMessage>();
        var rejected = new ConcurrentBag<string>();
        var retried = new ConcurrentBag<string>();
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(message);
        sut.StartSubscribing(true, (_) =>
        {
            var handled = rejected.Contains(_.Message);
            if (!handled)
                rejected.Add(_.Message);
            else
                retried.Add(_.Message);
            return Task.FromResult(handled);
        });
        message = _fixture.Create<PublishMessage>();
        await sut.PublishAsync(message);
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while ((retried.Count == 0) && (DateTime.UtcNow < timeout))
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        //sut.Stop(); // Don't stop transient queue or it will be deleted
        Assert.That(retried, Does.Contain(message.Message));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _context?.Dispose();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
