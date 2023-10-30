using Microservices.Shared.Mocks;
using Microservices.Shared.Queues.RabbitMQ.IntegrationTests.ApiModels;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Microservices.Shared.Queues.RabbitMQ.IntegrationTests
{
    internal class RabbitMQQueueTestsContext : IDisposable
    {
        private const string _baseUrl = "http://localhost:10672";
        private const string _auth = "YWRtaW46UEBzc3cwcmQ=";

        private readonly Fixture _fixture;
        private readonly string _vHost;
        private readonly RabbitMQQueueOptions _options;
        private readonly Mock<IOptions<RabbitMQQueueOptions>> _mockOptions;
        private readonly HttpClient _httpClient;

        private bool _disposedValue;

        internal string Suffix { get; }

        public RabbitMQQueueTestsContext()
        {
            /* These tests require a user and password to be created in the target RabbitMQ.
             * For example in the shell of the RabbitMQ container:
             *
             * rabbitmqctl add_user integration.tests password
             * rabbitmqctl set_user_tags integration.tests administrator
             * 
             * Every test context instance will create, use and delete its own virtual host in RabbitMQ.
             */

            _fixture = new();
            _vHost = _fixture.Create<string>();
            
            Suffix = _fixture.Create<string>();

            _options = new RabbitMQQueueOptions { Nodes = new[] { "localhost:10572", "localhost:10573" }, User = "integration.tests", Password = "password", VirtualHost = _vHost, SubscriberSuffix = Suffix, RetryDelayMilliseconds = 0 };
            _mockOptions = new(MockBehavior.Strict);
            _mockOptions.Setup(_ => _.Value).Returns(_options);

            _httpClient = new();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _auth);

            CreateVHostAsync().Wait();
        }

        internal RabbitMQQueue<TMessage> Sut<TMessage>() => new(_mockOptions.Object, new ConnectionFactory(), new MockLogger<RabbitMQQueue<TMessage>>().Object);

        internal async Task<string?> GetQueueBoundToExchangeAsync(string exchangeName)
        {
            var bindings = await GetEntitiesAsync<BindingModel>();
            var binding = bindings.FirstOrDefault(_ => (_.VHost == _vHost) && (_.Source == exchangeName) && (_.DestinationType == "queue"));
            return binding?.Destination;
        }

        internal RabbitMQQueueTestsContext WithRetryDelayMilliseconds(int ms)
        {
            _options.RetryDelayMilliseconds = ms;
            return this;
        }

        internal async Task AssertExchangeExistsAsync(string name, string type)
        {
            var exchanges = await GetEntitiesAsync<ExchangeModel>();
            Assert.That(exchanges, Has.Exactly(1).Matches<ExchangeModel>(_ => (_.VHost == _vHost) && (_.Name == name) && (_.Type == type)));
        }

        internal async Task AssertQueueExistsAsync(string queueName, bool durable, bool autoDelete)
        {
            var queues = await GetEntitiesAsync<QueueModel>();
            Assert.That(queues, Has.Exactly(1).Matches<QueueModel>(_ => (_.VHost == _vHost) && (_.Name == queueName) && (_.Durable == durable) && (_.AutoDelete == autoDelete)));
        }

        internal async Task AssertQueueMessageCount(string name, int count)
        {
            // The API may not return message information if called too quickly
            QueueModel? queue = null;
            var timeout = DateTime.UtcNow.AddSeconds(5);
            while (DateTime.UtcNow < timeout)
            {
                var queues = await GetEntitiesAsync<QueueModel>();
                queue = queues.FirstOrDefault(_ => (_.VHost == _vHost) && (_.Name == name));
                if (queue?.MessageCount == count)
                    break;
                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }
            Assert.That(queue?.MessageCount, Is.EqualTo(count));
        }

        internal async Task AssertBindingExistsAsync(string queueName, string exchangeName, string routingKey)
        {
            var bindings = await GetEntitiesAsync<BindingModel>();
            Assert.That(bindings, Has.Exactly(1).Matches<BindingModel>(_ => (_.VHost == _vHost) && (_.Source == exchangeName) && (_.Destination == queueName) && (_.DestinationType == "queue") && (_.RoutingKey == routingKey)));
        }

        internal async Task AssertDeadLetterExchangeExistsAsync(string queueName, string exchangeName)
        {
            var queues = await GetEntitiesAsync<QueueModel>();
            Assert.That(queues, Has.Exactly(1).Matches<QueueModel>(_ => (_.VHost == _vHost) && (_.Name == queueName) && (_.Arguments is not null) && (_.Arguments[Headers.XDeadLetterExchange]?.ToString() == exchangeName)));
        }

        private async Task CreateVHostAsync()
        {
            var apiUrl = $"{_baseUrl}/api/vhosts/{_vHost}";
            Debug.WriteLine($"Creating {_vHost} at {apiUrl}");
            var response = await _httpClient.PutAsync(apiUrl, new StringContent("", Encoding.UTF8, MediaTypeNames.Application.Json));
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to create VHOST");

            apiUrl = $"{_baseUrl}/api/permissions/{_vHost}/integration.tests";
            var requestBody = """{"configure":".*","write":".*","read":".*"}""";
            Debug.WriteLine($"Setting permissions {requestBody} at {apiUrl}");
            response = await _httpClient.PutAsync(apiUrl, new StringContent(requestBody, Encoding.UTF8, MediaTypeNames.Application.Json));
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to set permissions");
        }

        private async Task DeleteVHostAsync()
        {
            var apiUrl = $"{_baseUrl}/api/vhosts/{_vHost}";
            Debug.WriteLine($"Deleting {_vHost} at {apiUrl}");
            var response = await _httpClient.DeleteAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Failed to delete VHOST");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    DeleteVHostAsync().Wait();
                    _httpClient?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private async Task<T[]> GetEntitiesAsync<T>()
        {
            var entitiesName = $"{typeof(T).Name.Replace("Model", "").ToLower()}s";
            var apiUrl = $"{_baseUrl}/api/{entitiesName}";
            Debug.WriteLine($"Requesting {entitiesName} from {apiUrl}");
            var response = await _httpClient.GetStringAsync(apiUrl);
            return JsonSerializer.Deserialize<T[]>(response)!;
        }
    }
}
