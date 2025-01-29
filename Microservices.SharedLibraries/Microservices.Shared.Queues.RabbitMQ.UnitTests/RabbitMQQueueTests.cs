using Microservices.Shared.Queues.RabbitMQ.UnitTests.ApiModels;
using Microservices.Shared.Queues.RabbitMQ.UnitTests.QueueModels;
using RabbitMQ.Client;

namespace Microservices.Shared.Queues.RabbitMQ.UnitTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "RabbitMQ")]
public class RabbitMQQueueTests
{
    private readonly Fixture _fixture = new();
    private readonly RabbitMQQueueTestsContext _context = new();

    [Test]
    public void Constructor_sets_username()
    {
        using var sut = _context.Sut<SendMessage>();
        _context.AssertUsernameIsSet();
    }

    [Test]
    public void Constructor_sets_password()
    {
        using var sut = _context.Sut<SendMessage>();
        _context.AssertPasswordIsSet();
    }

    [Test]
    public void Constructor_sets_virtual_host()
    {
        using var sut = _context.Sut<SendMessage>();
        _context.AssertVirtualHostIsSet();
    }

    [Test]
    public async Task SendAsync_creates_a_durable_permanent_queue()
    {
        using var sut = _context.Sut<SendMessage>();
        await sut.SendAsync(_fixture.Create<SendMessage>());
        _context.AssertQueueCreatedOnce(nameof(SendMessage), durable: true, exclusive: false, autoDelete: false);
    }

    [Test]
    public async Task SendAsync_only_creates_queue_once()
    {
        using var sut = _context.Sut<SendMessage>();
        await sut.SendAsync(_fixture.Create<SendMessage>());
        await sut.SendAsync(_fixture.Create<SendMessage>());
        await sut.SendAsync(_fixture.Create<SendMessage>());
        _context.AssertQueueCreatedOnce(nameof(SendMessage), durable: true, exclusive: false, autoDelete: false);
    }

    [Test]
    public async Task SendAsync_sends_message()
    {
        var message = _fixture.Create<SendMessage>();
        using var sut = _context.Sut<SendMessage>();
        await sut.SendAsync(message);
        _context.AssertMessagePublished(string.Empty, nameof(SendMessage), message);
    }

    [Test]
    public async Task PublishAsync_creates_a_direct_exchange()
    {
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(_fixture.Create<PublishMessage>());
        _context.AssertExchangeCreatedOnce(nameof(PublishMessage), type: ExchangeType.Direct);
    }

    [Test]
    public async Task PublishAsync_sends_message_to_all_subscribers()
    {
        var message = _fixture.Create<SendMessage>();
        using var sut = _context.Sut<SendMessage>();
        await sut.PublishAsync(message);
        _context.AssertMessagePublished(nameof(SendMessage), "ALL", message);
    }

    [Test]
    public async Task PublishAsync_only_creates_exchange_once()
    {
        using var sut = _context.Sut<PublishMessage>();
        await sut.PublishAsync(_fixture.Create<PublishMessage>());
        await sut.PublishAsync(_fixture.Create<PublishMessage>());
        await sut.PublishAsync(_fixture.Create<PublishMessage>());
        _context.AssertExchangeCreatedOnce(nameof(PublishMessage), type: ExchangeType.Direct);
    }

    [Test]
    public void StartReceiving_reads_from_a_durable_permanent_queue()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartReceiving((_) => Task.FromResult(true));
        _context.AssertQueueCreatedOnce(nameof(SendMessage), durable: true, exclusive: false, autoDelete: false);
    }

    [Test]
    public void StartReceiving_consumes_queue()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartReceiving((_) => Task.FromResult(true));
        _context.AssertConsumingQueue(nameof(SendMessage));
    }

    [Test]
    public async Task StartReceiving_handled_message_sends_ack()
    {
        var message = _fixture.Create<SendMessage>();
        using var sut = _context.Sut<SendMessage>();
        sut.StartReceiving((_) => Task.FromResult(true));
        await sut.SendAsync(message);
        _context.AssertMessageAckd();
    }

    [Test]
    public async Task StartReceiving_unhandled_message_sends_nack_with_requeue()
    {
        var message = _fixture.Create<SendMessage>();
        using var sut = _context.Sut<SendMessage>();
        sut.StartReceiving((_) => Task.FromResult(false));
        await sut.SendAsync(message);
        _context.AssertMessageNackd(true);
    }

    [Test]
    public void StartReceiving_invalid_message_sends_nack_with_requeue()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartReceiving((_) => Task.FromResult(false));
        _context.SendInvalidMessage();
        _context.AssertMessageNackd(true);
    }

    [Test]
    public void StartReceiving_wrong_message_sends_nack_with_requeue()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartReceiving((_) => Task.FromResult(false));
        _context.SendWrongMessage();
        _context.AssertMessageNackd(true);
    }

    [Test]
    public void StartSubscribing_throws_if_already_receiving()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartReceiving((_) => Task.FromResult(false));
        Action action = () => sut.StartSubscribing(true, (_) => Task.FromResult(true));
        action.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void StartSubscribing_transient_reads_from_a_non_durable_autodelete_queue()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(true, (_) => Task.FromResult(true));
        _context.AssertQueueCreatedOnce(string.Empty, durable: false, exclusive: true, autoDelete: true);
    }

    [Test]
    public void StartSubscribing_transient_binds_to_exchange_with_no_headers()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(true, (_) => Task.FromResult(true));
        _context.AssertBindingWithoutHeaders(string.Empty, nameof(SendMessage), "ALL");
    }

    [Test]
    public void StartSubscribing_transient_consumes_queue()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(true, (_) => Task.FromResult(true));
        _context.AssertConsumingQueue(string.Empty);
    }

    [Test]
    public async Task StartSubscribing_transient_handled_message_sends_ack()
    {
        var message = _fixture.Create<SendMessage>();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(true, (_) => Task.FromResult(true));
        await sut.SendAsync(message);
        _context.AssertMessageAckd();
    }

    [Test]
    public async Task StartSubscribing_transient_unhandled_message_sends_nack_with_requeue()
    {
        var message = _fixture.Create<SendMessage>();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(true, (_) => Task.FromResult(false));
        await sut.SendAsync(message);
        _context.AssertMessageNackd(true);
    }

    [Test]
    public void StartSubscribing_transient_invalid_message_sends_nack_with_requeue()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(true, (_) => Task.FromResult(false));
        _context.SendInvalidMessage();
        _context.AssertMessageNackd(true);
    }

    [Test]
    public void StartSubscribing_transient_wrong_message_sends_nack_with_requeue()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(true, (_) => Task.FromResult(false));
        _context.SendWrongMessage();
        _context.AssertMessageNackd(true);
    }

    [Test]
    public void StartSubscribing_non_transient_without_retry_delay_reads_from_a_durable_permanent_queue()
    {
        _context.WithoutRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        _context.AssertQueueCreatedOnce(queueName, durable: true, exclusive: false, autoDelete: false);
    }

    [Test]
    public void StartSubscribing_non_transient_without_retry_delay_creates_queue_with_no_headers()
    {
        _context.WithoutRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        _context.AssertQueueHeaders(queueName, null);
    }

    [Test]
    public void StartSubscribing_non_transient_without_retry_delay_creates_one_exchange()
    {
        _context.WithoutRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        _context.AssertExchangeNames(nameof(SendMessage));
    }

    [Test]
    public void StartSubscribing_non_transient_without_retry_delay_creates_one_queue()
    {
        _context.WithoutRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        _context.AssertQueueNames(queueName);
    }

    [Test]
    public void StartSubscribing_non_transient_without_retry_delay_creates_one_binding()
    {
        _context.WithoutRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        _context.AssertBindings(new BindingModel(queueName, nameof(SendMessage), "ALL"));
    }

    [Test]
    public void StartSubscribing_non_transient_without_retry_delay_consumes_queue()
    {
        _context.WithoutRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        _context.AssertConsumingQueue(queueName);
    }

    [Test]
    public async Task StartSubscribing_non_transient_without_retry_delay_handled_message_sends_ack()
    {
        _context.WithoutRetryDelay();
        var message = _fixture.Create<SendMessage>();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        await sut.SendAsync(message);
        _context.AssertMessageAckd();
    }

    [Test]
    public async Task StartSubscribing_non_transient_without_retry_delay_unhandled_message_sends_nack_with_requeue()
    {
        _context.WithoutRetryDelay();
        var message = _fixture.Create<SendMessage>();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(false));
        await sut.SendAsync(message);
        _context.AssertMessageNackd(true);
    }

    [Test]
    public void StartSubscribing_non_transient_without_retry_delay_invalid_message_sends_nack_with_requeue()
    {
        _context.WithoutRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(false));
        _context.SendInvalidMessage();
        _context.AssertMessageNackd(true);
    }

    [Test]
    public void StartSubscribing_non_transient_without_retry_delay_wrong_message_sends_nack_with_requeue()
    {
        _context.WithoutRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(false));
        _context.SendWrongMessage();
        _context.AssertMessageNackd(true);
    }

    [Test]
    public void StartSubscribing_non_transient_with_retry_delay_reads_from_a_durable_permanent_queue()
    {
        _context.WithRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        _context.AssertQueueCreatedOnce(queueName, durable: true, exclusive: false, autoDelete: false);
    }

    [Test]
    public void StartSubscribing_non_transient_with_retry_creates_queue_with_dead_letter_headers()
    {
        _context.WithRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        var retryName = $"{queueName}-retry";
        _context.AssertQueueHeaders(queueName, new Dictionary<string, object>
        {
            [Headers.XDeadLetterExchange] = retryName,
            [Headers.XDeadLetterRoutingKey] = string.Empty
        });
    }

    [Test]
    public void StartSubscribing_non_transient_with_retry_creates_retry_queue_with_dead_letter_headers()
    {
        _context.WithRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        var retryName = $"{queueName}-retry";
        _context.AssertQueueHeaders(retryName, new Dictionary<string, object>
        {
            [Headers.XMessageTTL] = _context.RetryDelay,
            [Headers.XDeadLetterExchange] = nameof(SendMessage),
            [Headers.XDeadLetterRoutingKey] = _context.Suffix
        });
    }

    [Test]
    public void StartSubscribing_non_transient_with_retry_delay_creates_two_exchanges()
    {
        _context.WithRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        var retryName = $"{queueName}-retry";
        _context.AssertExchangeNames(nameof(SendMessage), retryName);
    }

    [Test]
    public void StartSubscribing_non_transient_with_retry_delay_creates_two_queues()
    {
        _context.WithRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        var retryName = $"{queueName}-retry";
        _context.AssertQueueNames(queueName, retryName);
    }

    [Test]
    public void StartSubscribing_non_transient_with_retry_delay_creates_three_bindings()
    {
        _context.WithRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        var retryName = $"{queueName}-retry";
        _context.AssertBindings(
            new BindingModel(queueName, nameof(SendMessage), "ALL"), // Shared
            new BindingModel(queueName, nameof(SendMessage), _context.Suffix), // Dedicated
            new BindingModel(retryName, retryName, string.Empty)); // Retry
    }
    
    [Test]
    public void StartSubscribing_non_transient_with_retry_delay_consumes_queue()
    {
        _context.WithRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        var queueName = $"{nameof(SendMessage)}-{_context.Suffix}";
        _context.AssertConsumingQueue(queueName);
    }

    [Test]
    public async Task StartSubscribing_non_transient_with_retry_delay_handled_message_sends_ack()
    {
        _context.WithRetryDelay();
        var message = _fixture.Create<SendMessage>();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(true));
        await sut.SendAsync(message);
        _context.AssertMessageAckd();
    }

    [Test]
    public async Task StartSubscribing_non_transient_with_retry_delay_unhandled_message_sends_nack_with_no_requeue()
    {
        _context.WithRetryDelay();
        var message = _fixture.Create<SendMessage>();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(false));
        await sut.SendAsync(message);
        _context.AssertMessageNackd(false);
    }

    [Test]
    public void StartSubscribing_non_transient_with_retry_delay_invalid_message_sends_nack_with_no_requeue()
    {
        _context.WithRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(false));
        _context.SendInvalidMessage();
        _context.AssertMessageNackd(false);
    }

    [Test]
    public void StartSubscribing_non_transient_with_retry_delay_wrong_message_sends_nack_with_no_requeue()
    {
        _context.WithRetryDelay();
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(false));
        _context.SendWrongMessage();
        _context.AssertMessageNackd(false);
    }

    [Test]
    public void Stop_cancels()
    {
        using var sut = _context.Sut<SendMessage>();
        sut.StartSubscribing(false, (_) => Task.FromResult(false));
        sut.Stop();
        _context.AssertChannelCancelled();
    }
}
