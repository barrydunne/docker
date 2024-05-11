using Microservices.Shared.Mocks;
using Microservices.Shared.Queues.RabbitMQ.UnitTests.ApiModels;
using Microservices.Shared.Queues.RabbitMQ.UnitTests.QueueModels;
using Microsoft.Extensions.Options;
using NSubstitute;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace Microservices.Shared.Queues.RabbitMQ.UnitTests;

internal class RabbitMQQueueTestsContext
{
    private readonly Fixture _fixture;
    private readonly string _vHost;
    private readonly RabbitMQQueueOptions _options;
    private readonly IOptions<RabbitMQQueueOptions> _mockOptions;
    private readonly ConcurrentBag<QueueModel> _queues;
    private readonly ConcurrentBag<ExchangeModel> _exchanges;
    private readonly ConcurrentBag<PublishModel> _published;
    private readonly ConcurrentBag<BindingModel> _bindings;
    private readonly ConcurrentBag<IBasicConsumer> _consumers;
    private readonly IBasicProperties _mockBasicProperties;
    private readonly IModel _mockModel;
    private readonly IConnection _mockConnection;
    private readonly IConnectionFactory _mockConnectionFactory;

    internal string Suffix => _options.SubscriberSuffix;
    internal int RetryDelay => _options.RetryDelayMilliseconds;

    public RabbitMQQueueTestsContext()
    {
        _fixture = new();
        _vHost = _fixture.Create<string>();

        _options = _fixture.Create<RabbitMQQueueOptions>();
        _mockOptions = Substitute.For<IOptions<RabbitMQQueueOptions>>();
        _mockOptions
            .Value
            .Returns(callInfo => _options);

        _queues = new();
        _exchanges = new();
        _published = new();
        _bindings = new();
        _consumers = new();

        _mockBasicProperties = Substitute.For<IBasicProperties>();

        _mockModel = Substitute.For<IModel>();
        _mockModel
            .QueueDeclare(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>(), Arg.Any<bool>(), Arg.Any<IDictionary<string, object>>())
            .Returns(callInfo =>
            {
                var queue = callInfo.Arg<string>();
                return new QueueDeclareOk(queue, 0, 0);
            })
            .AndDoes(callInfo => 
            {
                var queue = callInfo.ArgAt<string>(0);
                var durable = callInfo.ArgAt<bool>(1);
                var exclusive = callInfo.ArgAt<bool>(2);
                var autoDelete = callInfo.ArgAt<bool>(3);
                var arguments = callInfo.ArgAt<IDictionary<string, object>>(4);
                _queues.Add(new(queue, durable, exclusive, autoDelete, arguments));
            });
        _mockModel
            .When(_ => _.ExchangeDeclare(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>(), Arg.Any<IDictionary<string, object>>()))
            .Do(callInfo =>
            {
                var exchange = callInfo.ArgAt<string>(0);
                var type = callInfo.ArgAt<string>(1);
                var durable = callInfo.ArgAt<bool>(2);
                var autoDelete = callInfo.ArgAt<bool>(3);
                var arguments = callInfo.ArgAt<IDictionary<string, object>>(4);

                _exchanges.Add(new(exchange, type, durable, autoDelete, arguments));
            });
        _mockModel
            .CreateBasicProperties()
            .Returns(callInfo => _mockBasicProperties);
        _mockModel
            .When(_ => _.BasicPublish(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<IBasicProperties>(), Arg.Any<ReadOnlyMemory<byte>>()))
            .Do(callInfo => 
            {
                var exchange = callInfo.ArgAt<string>(0);
                var routingKey = callInfo.ArgAt<string>(1);
                var mandatory = callInfo.ArgAt<bool>(2);
                var basicProperties = callInfo.ArgAt<IBasicProperties>(3);
                var body = callInfo.ArgAt<ReadOnlyMemory<byte>>(4);

                _published.Add(new(exchange, routingKey, mandatory, basicProperties, body));

                foreach (var consumer in _consumers)
                    (consumer as EventingBasicConsumer)?.HandleBasicDeliver(_fixture.Create<string>(), _fixture.Create<ulong>(), false, exchange, routingKey, basicProperties, body);
            });
        _mockModel
            .BasicConsume(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>(), Arg.Any<IDictionary<string, object>>(), Arg.Do<IBasicConsumer>(consumer => _consumers.Add(consumer)))
            .Returns(callInfo => _fixture.Create<string>());
        _mockModel
            .When(_ => _.QueueBind(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IDictionary<string, object>>()))
            .Do(callInfo => 
            {
                var queue = callInfo.ArgAt<string>(0);
                var exchange = callInfo.ArgAt<string>(1);
                var routingKey = callInfo.ArgAt<string>(2);

                _bindings.Add(new(queue, exchange, routingKey));
            });

        _mockConnection = Substitute.For<IConnection>();
        _mockConnection
            .CreateModel()
            .Returns(callInfo => _mockModel);

        _mockConnectionFactory = Substitute.For<IConnectionFactory>();
        _mockConnectionFactory
            .CreateConnection(Arg.Any<List<AmqpTcpEndpoint>>())
            .Returns(callInfo => _mockConnection);
    }

    internal RabbitMQQueue<TMessage> Sut<TMessage>() => new(_mockOptions, _mockConnectionFactory, new MockLogger<RabbitMQQueue<TMessage>>());

    internal void SendInvalidMessage()
    {
        foreach (var consumer in _consumers)
            (consumer as EventingBasicConsumer)?.HandleBasicDeliver(_fixture.Create<string>(), _fixture.Create<ulong>(), false, _fixture.Create<string>(), _fixture.Create<string>(), _mockBasicProperties, Encoding.UTF8.GetBytes("INVALID"));
    }

    internal void SendWrongMessage()
    {
        foreach (var consumer in _consumers)
            (consumer as EventingBasicConsumer)?.HandleBasicDeliver(_fixture.Create<string>(), _fixture.Create<ulong>(), false, _fixture.Create<string>(), _fixture.Create<string>(), _mockBasicProperties, Encoding.UTF8.GetBytes("""{"UnexpectedType":1}"""));
    }

    internal RabbitMQQueueTestsContext WithoutRetryDelay()
    {
        _options.RetryDelayMilliseconds = 0;
        return this;
    }

    internal RabbitMQQueueTestsContext WithRetryDelay()
    {
        while (_options.RetryDelayMilliseconds < 1)
            _options.RetryDelayMilliseconds = _fixture.Create<int>();
        return this;
    }

    internal RabbitMQQueueTestsContext AssertUsernameIsSet()
    {
        _mockConnectionFactory.Received(1).UserName = _options.User;
        return this;
    }

    internal RabbitMQQueueTestsContext AssertPasswordIsSet()
    {
        _mockConnectionFactory.Received(1).Password = _options.Password;
        return this;
    }

    internal RabbitMQQueueTestsContext AssertVirtualHostIsSet()
    {
        _mockConnectionFactory.Received(1).VirtualHost = _options.VirtualHost;
        return this;
    }

    internal RabbitMQQueueTestsContext AssertQueueCreatedOnce(string queue, bool durable, bool exclusive, bool autoDelete)
    {
        Assert.That(_queues, Has.Exactly(1).Matches<QueueModel>(_ => (_.Queue == queue) && (_.Durable == durable) && (_.Exclusive == exclusive) && (_.AutoDelete == autoDelete)));
        return this;
    }

    internal RabbitMQQueueTestsContext AssertExchangeCreatedOnce(string exchange, string type)
    {
        Assert.That(_exchanges, Has.Exactly(1).Matches<ExchangeModel>(_ => (_.Exchange == exchange) && (_.Type == type)));
        return this;
    }

    internal RabbitMQQueueTestsContext AssertMessagePublished(string exchange, string routingKey, SendMessage message)
    {
        Assert.That(_published, Has.Exactly(1).Matches<PublishModel>(_ => (_.Exchange == exchange) && (_.RoutingKey == routingKey) && (Encoding.UTF8.GetString(_.Body.ToArray()) == JsonSerializer.Serialize(message))));
        return this;
    }

    internal RabbitMQQueueTestsContext AssertConsumingQueue(string queue)
    {
        _mockModel.Received(1).BasicConsume(queue, Arg.Any<bool>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>(), Arg.Any<Dictionary<string, object>>(), Arg.Any<IBasicConsumer>());
        return this;
    }

    internal RabbitMQQueueTestsContext AssertMessageAckd()
    {
        _mockModel.Received(1).BasicAck(Arg.Any<ulong>(), false);
        return this;
    }

    internal RabbitMQQueueTestsContext AssertMessageNackd(bool requeue)
    {
        _mockModel.Received(1).BasicNack(Arg.Any<ulong>(), false, requeue);
        return this;
    }

    internal RabbitMQQueueTestsContext AssertBindingWithoutHeaders(string queue, string exchange, string routingKey)
    {
        Assert.That(_bindings, Has.Exactly(1).Matches<BindingModel>(_ => (_.Queue == queue) && (_.Exchange == exchange) && (_.RoutingKey == routingKey)));
        return this;
    }

    internal RabbitMQQueueTestsContext AssertQueueHeaders(string queueName, IDictionary<string, object>? arguments)
    {
        var queue = _queues.FirstOrDefault(_ => _.Queue == queueName);
        Assert.That(queue, Is.Not.Null, "Queue not found");
        if (arguments is null)
            Assert.That(queue!.Arguments, Is.Null, "Different arguments");
        else
        {
            Assert.That(arguments.Keys.ToArray(), Is.EquivalentTo(queue!.Arguments!.Keys.ToArray()), "Different keys");
            foreach (var key in arguments.Keys)
                Assert.That(arguments[key], Is.EqualTo(queue.Arguments![key]), $"Different {key} value");
        }
        return this;
    }

    internal RabbitMQQueueTestsContext AssertExchangeNames(params string[] exchangeNames)
    {
        Assert.That(exchangeNames, Is.EquivalentTo(_exchanges.Select(_ => _.Exchange)));
        return this;
    }

    internal RabbitMQQueueTestsContext AssertQueueNames(params string[] queueNames)
    {
        Assert.That(queueNames, Is.EquivalentTo(_queues.Select(_ => _.Queue)));
        return this;
    }

    internal RabbitMQQueueTestsContext AssertBindings(params BindingModel[] bindingModels)
    {
        Assert.That(bindingModels, Is.EquivalentTo(_bindings));
        return this;
    }

    internal RabbitMQQueueTestsContext AssertChannelCancelled()
    {
        _mockModel.Received(1).BasicCancel(Arg.Any<string>());
        return this;
    }
}
