using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Microservices.Shared.Queues.RabbitMQ;

/// <summary>
/// Provides interaction with a messages on RabbitMQ.
/// </summary>
/// <typeparam name="TMessage">The type of message for this queue.</typeparam>
public class RabbitMQQueue<TMessage> : IQueue<TMessage>, IDisposable
{
    private const string _allRoutingKey = "ALL";

    private readonly RabbitMQQueueOptions _options;
    private readonly List<AmqpTcpEndpoint> _endpoints;
    private readonly ILogger _logger;
    private readonly IConnectionFactory _factory;
    private readonly string _typeName;

    private IConnection? _connection;
    private IModel? _channel;
    private bool _queueCreated;
    private bool _exchangeCreated;
    private string _queueName;
    private string? _consumerTag;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQQueue{TMessage}"/> class.
    /// </summary>
    /// <param name="options">The connection configuration options.</param>
    /// <param name="connectionFactory">The factory to use to create RabbitMQ connections.</param>
    /// <param name="logger">The logger to write to.</param>
    public RabbitMQQueue(IOptions<RabbitMQQueueOptions> options, IConnectionFactory connectionFactory, ILogger<RabbitMQQueue<TMessage>> logger)
    {
        _options = options.Value;
        _endpoints = _options.Nodes.Select(_ => new AmqpTcpEndpoint(new Uri($"amqp://{_}"))).ToList();

        _factory = connectionFactory;
        _factory.UserName = _options.User;
        _factory.Password = _options.Password;
        _factory.VirtualHost = _options.VirtualHost;
        _logger = logger;
        _typeName = typeof(TMessage).Name;
        _queueName = _typeName;

        _connection = null;
        _channel = null;
        _queueCreated = false;
        _exchangeCreated = false;
        _consumerTag = null;
    }

    /// <inheritdoc/>
    public Task SendAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureQueueCreated();
        Publish(string.Empty, _typeName, message);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task PublishAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureExchangeCreated();
        Publish(_typeName, string.Empty, message);
        return Task.CompletedTask;
    }

    private void Publish(string exchange, string queue, TMessage message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel!.BasicPublish(
            exchange: exchange,
            routingKey: string.IsNullOrWhiteSpace(exchange) ? queue : _allRoutingKey,
            basicProperties: null,
            body: body);

        // In this sample, use the simplest approach to publishing with confirms, that is, publishing a message and waiting synchronously for its confirmation.
        // WaitForConfirmsOrDie() returns as soon as the message has been confirmed.
        // If the message is not confirmed within the timeout or if it is nack-ed (meaning the broker could not take care of it for some reason), the method will throw an exception.
        // In a production system this would slow down publishing and would need to be implemented asynchronously to not block publishing.
        // See https://www.rabbitmq.com/tutorials/tutorial-seven-dotnet.html
        _channel!.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
    }

    /// <inheritdoc/>
    public void StartReceiving(Func<TMessage, Task<bool>> handler)
    {
        EnsureQueueCreated();
        StartProcessing(handler);
    }

    /// <inheritdoc/>
    public void StartSubscribing(bool transientSubscription, Func<TMessage, Task<bool>> handler)
    {
        if (_queueCreated)
            throw new InvalidOperationException("Cannot start subscribing after sending or receiving");

        EnsureExchangeCreated();
        EnsureQueueCreated(transientSubscription, true, transientSubscription ? 0 : _options.RetryDelayMilliseconds);
        StartProcessing(handler, transientSubscription ? 0 : _options.RetryDelayMilliseconds);
    }

    /// <inheritdoc/>
    public void Stop()
    {
        if (_consumerTag is not null)
            _channel!.BasicCancel(_consumerTag);
    }

    private void StartProcessing(Func<TMessage, Task<bool>> handler, int retryDelayMilliseconds = 0)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, e) =>
        {
            try
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var payload = JsonSerializer.Deserialize<TMessage>(message)!;
                _logger.LogInformation("Received {MessageType} message", _typeName);
                var handled = handler.Invoke(payload).Result;
                if (handled)
                {
                    _channel!.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle event");
            }
            var requeue = retryDelayMilliseconds < 1;
            _channel!.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: requeue);
        };
        _consumerTag = _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
    }

    private void EnsureConnectionCreated()
    {
        if (_connection is null)
            _logger.LogDebug("RabbitMQQueue<{MessageType}> Create connection.", _typeName);
        _connection ??= _factory.CreateConnection(_endpoints);
    }

    private void EnsureChannelCreated()
    {
        EnsureConnectionCreated();
        if (_channel is null)
            _logger.LogDebug("RabbitMQQueue<{MessageType}>Create channel.", _typeName);
        _channel ??= _connection!.CreateModel();
    }

    private void EnsureQueueCreated(bool temporaryQueue = false, bool bindToExchange = false, int retryDelayMilliseconds = 0)
    {
        EnsureChannelCreated();
        lock (_channel!)
        {
            if (_queueCreated)
                return;

            if (temporaryQueue)
            {
                _logger.LogInformation("Using temporary queue {QueueName}", _queueName);
                _queueName = _channel.QueueDeclare().QueueName;
            }
            else
            {
                if (bindToExchange)
                    _queueName = $"{_queueName}-{_options.SubscriberSuffix}";

                _logger.LogInformation("Using queue {QueueName}", _queueName);

                if (bindToExchange && (retryDelayMilliseconds > 0))
                {
                    /* Retries will work as follows
                     *   MSG -> EXCHANGE[_typeName](routing:ALL)                <- Normal publish
                     *   EXCHANGE[_typeName] -> QUEUE[_queueName](routing:ALL)  <- Normal subscribe
                     * After nack with requeue=false:
                     *   QUEUE[_queueName] -> EXCHANGE[_queueName-retry]
                     *   EXCHANGE[_queueName-retry] -> QUEUE[_queueName-retry]
                     * After TTL
                     *   QUEUE[_queueName-retry] -> EXCHANGE[_typeName](routing:suffix)
                     *   EXCHANGE[_typeName] -> QUEUE[_queueName](routing:suffix)
                     */

                    var retryName = $"{_queueName}-retry";

                    // Configure
                    //   QUEUE[_queueName] -> EXCHANGE[_queueName-retry]
                    _channel.ExchangeDeclare(exchange: retryName, type: ExchangeType.Direct);
                    _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
                    {
                        [Headers.XDeadLetterExchange] = retryName,
                        [Headers.XDeadLetterRoutingKey] = string.Empty
                    });

                    // Configure
                    //   EXCHANGE[_queueName-retry] -> QUEUE[_queueName-retry]
                    //   QUEUE[_queueName-retry] -> EXCHANGE[_typeName](routing:suffix)
                    _channel.QueueDeclare(queue: retryName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
                    {
                        [Headers.XMessageTTL] = retryDelayMilliseconds,
                        [Headers.XDeadLetterExchange] = _typeName,
                        [Headers.XDeadLetterRoutingKey] = _options.SubscriberSuffix
                    });
                    _channel.QueueBind(retryName, exchange: retryName, routingKey: string.Empty);

                    // Configure
                    //   EXCHANGE[_typeName] -> QUEUE[_queueName](routing:suffix)
                    _channel.QueueBind(_queueName, exchange: _typeName, routingKey: _options.SubscriberSuffix);
                }
                else
                    _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            }

            if (bindToExchange)
                _channel.QueueBind(_queueName, exchange: _typeName, routingKey: _allRoutingKey);

            // Marking messages as persistent doesn't fully guarantee that a message won't be lost.
            // Although it tells RabbitMQ to save the message to disk, there is still a short time window when RabbitMQ has accepted a message and hasn't saved it yet.
            _channel.CreateBasicProperties().Persistent = true;

            // Prevent basic round robin dispatching of messages to consumers and distribute work to idle consumers.
            // Don't dispatch a new message to a worker until it has processed and acknowledged the previous one.
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            // Use publisher confirms to make sure published messages have safely reached the broker.
            _channel.ConfirmSelect();

            _queueCreated = true;
        }
    }

    private void EnsureExchangeCreated()
    {
        EnsureChannelCreated();
        lock (_channel!)
        {
            if (_exchangeCreated)
                return;

            _channel!.ExchangeDeclare(exchange: _typeName, type: ExchangeType.Direct);

            // Use publisher confirms to make sure published messages have safely reached the broker.
            _channel.ConfirmSelect();

            _exchangeCreated = true;
        }
    }

    /// <summary>
    /// Dispose of this processor.
    /// </summary>
    /// <param name="disposing">Whether to dispose of resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _logger.LogDebug("RabbitMQQueue<{MessageType}> Disposing connection and channel.", _typeName);
                _connection?.Close(TimeSpan.FromSeconds(1));
                _connection?.Dispose();
                _channel?.Close();
                _channel?.Dispose();
            }
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
