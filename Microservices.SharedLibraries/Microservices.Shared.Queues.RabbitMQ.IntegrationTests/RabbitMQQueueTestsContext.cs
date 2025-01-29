using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microservices.Shared.Mocks;
using Microservices.Shared.Queues.RabbitMQ.IntegrationTests.ApiModels;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Microservices.Shared.Queues.RabbitMQ.IntegrationTests;

internal class RabbitMQQueueTestsContext : IDisposable
{
    private const string _auth = "YWRtaW46UEBzc3cwcmQ=";

    private readonly IContainer _containerNode1;
    private readonly IContainer _containerNode2;
    private readonly Fixture _fixture;
    private readonly string _vHost;
    private readonly RabbitMQQueueOptions _options;
    private readonly IOptions<RabbitMQQueueOptions> _mockOptions;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    private bool _disposedValue;

    internal string Suffix { get; }

    public RabbitMQQueueTestsContext()
    {
        var network = new NetworkBuilder()
            .WithName($"RabbitMQQueueTests_{Guid.NewGuid():N}")
            .Build();

        _containerNode1 = CreateRabbitNode(1, network);
        _containerNode1.StartAsync().GetAwaiter().GetResult();
        _containerNode2 = CreateRabbitNode(2, network);
        _containerNode2.StartAsync().GetAwaiter().GetResult();

        /* These tests require a user and password to be created in the target RabbitMQ.
         * For example in the shell of the RabbitMQ container:
         *
         * rabbitmqctl add_user integration.tests password
         * rabbitmqctl set_user_tags integration.tests administrator
         * 
         * Every test context instance will create, use and delete its own virtual host in RabbitMQ.
         */

        _containerNode1.ExecAsync(["rabbitmqctl", "add_user", "integration.tests", "password"]).GetAwaiter().GetResult();
        _containerNode1.ExecAsync(["rabbitmqctl", "set_user_tags", "integration.tests", "administrator"]).GetAwaiter().GetResult();

        _fixture = new();
        _baseUrl = $"http://localhost:{_containerNode1.GetMappedPublicPort(15672)}";
        _vHost = _fixture.Create<string>();
        
        Suffix = _fixture.Create<string>();

        _options = new RabbitMQQueueOptions 
        { 
            Nodes = new[] 
            { 
                $"localhost:{_containerNode1.GetMappedPublicPort(5672)}",
                $"localhost:{_containerNode2.GetMappedPublicPort(5672)}"
            },
            User = "integration.tests",
            Password = "password",
            VirtualHost = _vHost,
            SubscriberSuffix = Suffix,
            RetryDelayMilliseconds = 0
        };
        _mockOptions = Substitute.For<IOptions<RabbitMQQueueOptions>>();
        _mockOptions
            .Value
            .Returns(callInfo => _options);

        _httpClient = new();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _auth);

        CreateVHostAsync().Wait();
    }

    private IContainer CreateRabbitNode(int node, DotNet.Testcontainers.Networks.INetwork network)
    {
        using var mem = new MemoryStream();
        using var source = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microservices.Shared.Queues.RabbitMQ.IntegrationTests.rabbit-container-script.txt");
        source!.CopyTo(mem);
        var script = mem.ToArray().Where(_ => _ != 13).ToArray();

        var builder = new ContainerBuilder()
            .WithImage("rabbitmq:4.0.5-management")
            .WithName($"RabbitMQQueueTests.Node{node}_{Guid.NewGuid():N}")
            .WithPortBinding(5672, true)
            .WithPortBinding(15672, true)
            .WithNetwork(network)
            .WithHostname($"rabbitmq{node}")
            .WithNetworkAliases($"rabbitmq{node}")
            .WithEnvironment("RABBITMQ_DEFAULT_USER", "admin")
            .WithEnvironment("RABBITMQ_DEFAULT_PASS", "P@ssw0rd")
            .WithEnvironment("RABBITMQ_DEFAULT_VHOST", "microservices")
            .WithEnvironment($"RABBITMQ_NODENAME", $"rabbit{node}")
            .WithEnvironment("RABBITMQ_ERLANG_COOKIE", "2fde70c0-3606-4576-b83f-85e964f66f8d")
            .WithResourceMapping(script, "/opt/rabbitmq/start-cluster.sh", UnixFileModes.UserExecute)
            .WithCommand("/opt/rabbitmq/start-cluster.sh");

        if (node == 2)
        {
            builder = builder
                .WithEnvironment("RABBITMQ_JOIN_NODE", "rabbit1@rabbitmq1")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Background job with PID .*"));
        }
        else
            builder = builder.WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(15672));

        return builder.Build();
    }

    internal RabbitMQQueue<TMessage> Sut<TMessage>() => new(_mockOptions, new ConnectionFactory(), new MockLogger<RabbitMQQueue<TMessage>>());

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
        exchanges.Where(_ => (_.VHost == _vHost) && (_.Name == name) && (_.Type == type)).ShouldHaveSingleItem();
    }

    internal async Task AssertQueueExistsAsync(string queueName, bool durable, bool autoDelete)
    {
        var queues = await GetEntitiesAsync<QueueModel>();
        queues.Where(_ => (_.VHost == _vHost) && (_.Name == queueName) && (_.Durable == durable) && (_.AutoDelete == autoDelete)).ShouldHaveSingleItem();
    }

    internal async Task AssertQueueMessageCount(string name, int count)
    {
        // There can be significant latency in getting accurate queue statistics from the RabbitMQ Management API.
        // This sometimes results in message counts being unavailable or outdated for several seconds.
        QueueModel? queue = null;
        var timeout = DateTime.UtcNow.AddSeconds(30);
        while (DateTime.UtcNow < timeout)
        {
            var queues = await GetEntitiesAsync<QueueModel>();
            queue = queues.FirstOrDefault(_ => (_.VHost == _vHost) && (_.Name == name));
            if (queue?.MessageCount == count)
                break;
            await Task.Delay(TimeSpan.FromMilliseconds(250));
        }
        queue?.MessageCount.ShouldBe(count);
    }

    internal async Task AssertBindingExistsAsync(string queueName, string exchangeName, string routingKey)
    {
        var bindings = await GetEntitiesAsync<BindingModel>();
        bindings.Where(_ => (_.VHost == _vHost) && (_.Source == exchangeName) && (_.Destination == queueName) && (_.DestinationType == "queue") && (_.RoutingKey == routingKey)).ShouldHaveSingleItem();
    }

    internal async Task AssertDeadLetterExchangeExistsAsync(string queueName, string exchangeName)
    {
        var queues = await GetEntitiesAsync<QueueModel>();
        queues.Where(_ => (_.VHost == _vHost) && (_.Name == queueName) && (_.Arguments is not null) && (_.Arguments[Headers.XDeadLetterExchange]?.ToString() == exchangeName)).ShouldHaveSingleItem();
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
                _containerNode2.DisposeAsync().GetAwaiter().GetResult();
                _containerNode1.DisposeAsync().GetAwaiter().GetResult();
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
