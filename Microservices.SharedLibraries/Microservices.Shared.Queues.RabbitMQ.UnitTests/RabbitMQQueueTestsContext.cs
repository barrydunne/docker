using Microservices.Shared.Mocks;
using Microservices.Shared.Queues.RabbitMQ.UnitTests.ApiModels;
using Microservices.Shared.Queues.RabbitMQ.UnitTests.QueueModels;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace Microservices.Shared.Queues.RabbitMQ.UnitTests
{
    internal class RabbitMQQueueTestsContext
    {
        private readonly Fixture _fixture;
        private readonly string _vHost;
        private readonly RabbitMQQueueOptions _options;
        private readonly Mock<IOptions<RabbitMQQueueOptions>> _mockOptions;
        private readonly ConcurrentBag<QueueModel> _queues;
        private readonly ConcurrentBag<ExchangeModel> _exchanges;
        private readonly ConcurrentBag<PublishModel> _published;
        private readonly ConcurrentBag<BindingModel> _bindings;
        private readonly ConcurrentBag<IBasicConsumer> _consumers;
        private readonly Mock<IBasicProperties> _mockBasicProperties;
        private readonly Mock<IModel> _mockModel;
        private readonly Mock<IConnection> _mockConnection;
        private readonly Mock<IConnectionFactory> _mockConnectionFactory;

        internal string Suffix => _options.SubscriberSuffix;
        internal int RetryDelay => _options.RetryDelayMilliseconds;

        public RabbitMQQueueTestsContext()
        {
            _fixture = new();
            _vHost = _fixture.Create<string>();

            _options = _fixture.Create<RabbitMQQueueOptions>();
            _mockOptions = new(MockBehavior.Strict);
            _mockOptions.Setup(_ => _.Value).Returns(_options);

            _queues = new();
            _exchanges = new();
            _published = new();
            _bindings = new();
            _consumers = new();

            _mockBasicProperties = new();
            _mockBasicProperties.SetupSet(_ => _.Persistent = It.IsAny<bool>()).Verifiable();

            _mockModel = new(MockBehavior.Strict);
            _mockModel.Setup(_ => _.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<Dictionary<string, object>>()))
                .Callback((string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object>? arguments) => _queues.Add(new(queue, durable, exclusive, autoDelete, arguments)))
                .Returns((string queue, bool _, bool _, bool _, IDictionary<string, object> _) => new QueueDeclareOk(queue, 0, 0));
            _mockModel.Setup(_ => _.ExchangeDeclare(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<Dictionary<string, object>>()))
                .Callback((string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments) => _exchanges.Add(new(exchange, type, durable, autoDelete, arguments)));
            _mockModel.Setup(_ => _.CreateBasicProperties()).Returns(() => _mockBasicProperties.Object);
            _mockModel.Setup(_ => _.BasicQos(It.IsAny<uint>(), It.IsAny<ushort>(), It.IsAny<bool>())).Verifiable();
            _mockModel.Setup(_ => _.ConfirmSelect()).Verifiable();
            _mockModel.Setup(_ => _.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()))
                .Callback((string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, ReadOnlyMemory<byte> body) =>
                {
                    _published.Add(new(exchange, routingKey, mandatory, basicProperties, body));
                    foreach (var consumer in _consumers)
                        (consumer as EventingBasicConsumer)?.HandleBasicDeliver(_fixture.Create<string>(), _fixture.Create<ulong>(), false, exchange, routingKey, basicProperties, body);
                });
            _mockModel.Setup(_ => _.WaitForConfirmsOrDie(It.IsAny<TimeSpan>())).Verifiable();
            _mockModel.Setup(_ => _.BasicConsume(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<IBasicConsumer>()))
                .Callback((string _, bool _, string _, bool _, bool _, IDictionary<string, object> _, IBasicConsumer consumer) => _consumers.Add(consumer))
                .Returns(() => _fixture.Create<string>());
            _mockModel.Setup(_ => _.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()))
                .Callback((string queue, string exchange, string routingKey, IDictionary<string, object> _) => _bindings.Add(new(queue, exchange, routingKey)));
            _mockModel.Setup(_ => _.BasicAck(It.IsAny<ulong>(), It.IsAny<bool>())).Verifiable();
            _mockModel.Setup(_ => _.BasicNack(It.IsAny<ulong>(), It.IsAny<bool>(), It.IsAny<bool>())).Verifiable();
            _mockModel.Setup(_ => _.BasicCancel(It.IsAny<string>())).Verifiable();
            _mockModel.Setup(_ => _.Close()).Verifiable();
            _mockModel.Setup(_ => _.Dispose()).Verifiable();

            _mockConnection = new(MockBehavior.Strict);
            _mockConnection.Setup(_ => _.CreateModel()).Returns(_mockModel.Object);
            _mockConnection.Setup(_ => _.Close(It.IsAny<TimeSpan>())).Verifiable();
            _mockConnection.Setup(_ => _.Dispose()).Verifiable();

            _mockConnectionFactory = new(MockBehavior.Strict);
            _mockConnectionFactory.SetupSet(_ => _.Uri = It.IsAny<Uri>()).Verifiable();
            _mockConnectionFactory.SetupSet(_ => _.UserName = It.IsAny<string>()).Verifiable();
            _mockConnectionFactory.SetupSet(_ => _.Password = It.IsAny<string>()).Verifiable();
            _mockConnectionFactory.SetupSet(_ => _.VirtualHost = It.IsAny<string>()).Verifiable();
            _mockConnectionFactory.Setup(_ => _.CreateConnection(It.IsAny<List<AmqpTcpEndpoint>>())).Returns(_mockConnection.Object);
        }

        internal RabbitMQQueue<TMessage> Sut<TMessage>() => new(_mockOptions.Object, _mockConnectionFactory.Object, new MockLogger<RabbitMQQueue<TMessage>>().Object);

        internal void SendInvalidMessage()
        {
            foreach (var consumer in _consumers)
                (consumer as EventingBasicConsumer)?.HandleBasicDeliver(_fixture.Create<string>(), _fixture.Create<ulong>(), false, _fixture.Create<string>(), _fixture.Create<string>(), _mockBasicProperties.Object, Encoding.UTF8.GetBytes("INVALID"));
        }

        internal void SendWrongMessage()
        {
            foreach (var consumer in _consumers)
                (consumer as EventingBasicConsumer)?.HandleBasicDeliver(_fixture.Create<string>(), _fixture.Create<ulong>(), false, _fixture.Create<string>(), _fixture.Create<string>(), _mockBasicProperties.Object, Encoding.UTF8.GetBytes("""{"UnexpectedType":1}"""));
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
            _mockConnectionFactory.VerifySet(_ => _.UserName = _options.User, Times.Once);
            return this;
        }

        internal RabbitMQQueueTestsContext AssertPasswordIsSet()
        {
            _mockConnectionFactory.VerifySet(_ => _.Password = _options.Password, Times.Once);
            return this;
        }

        internal RabbitMQQueueTestsContext AssertVirtualHostIsSet()
        {
            _mockConnectionFactory.VerifySet(_ => _.VirtualHost = _options.VirtualHost, Times.Once);
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
            _mockModel.Verify(_ => _.BasicConsume(queue, It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<IBasicConsumer>()), Times.Once);
            return this;
        }

        internal RabbitMQQueueTestsContext AssertMessageAckd()
        {
            _mockModel.Verify(_ => _.BasicAck(It.IsAny<ulong>(), false), Times.Once);
            return this;
        }

        internal RabbitMQQueueTestsContext AssertMessageNackd(bool requeue)
        {
            _mockModel.Verify(_ => _.BasicNack(It.IsAny<ulong>(), false, requeue), Times.Once);
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
                Assert.That(queue.Arguments, Is.Null, "Different arguments");
            else
            {
                Assert.That(arguments.Keys.ToArray(), Is.EquivalentTo(queue.Arguments?.Keys.ToArray()), "Different keys");
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
            _mockModel.Verify(_ => _.BasicCancel(It.IsAny<string>()), Times.Once);
            return this;
        }
    }
}
