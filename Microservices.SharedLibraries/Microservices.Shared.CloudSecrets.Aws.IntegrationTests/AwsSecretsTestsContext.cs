using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using AutoFixture;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using System.Net;
using System.Text.Json;

namespace Microservices.Shared.CloudSecrets.Aws.IntegrationTests;

internal class AwsSecretsTestsContext : IDisposable
{
    private readonly IContainer _container;
    private readonly IAmazonSecretsManager _amazonSecretsManager;
    private readonly MockLogger<AwsSecrets> _mockLogger;
    private readonly Fixture _fixture;
    private readonly string _vault;
    private readonly string _secret;
    private readonly string _value;
    private bool _disposedValue;

    internal AwsSecrets Sut => new(_amazonSecretsManager, _mockLogger);

    internal string KnownVault => _vault;
    internal string KnownSecret => _secret;
    internal string KnownValue => _value;

    internal AwsSecretsTestsContext()
    {
        // Run a LocalStack container. Bind port 4566 to random local port, and wait for HTTP site to be available.
        _container = new ContainerBuilder()
            .WithImage("localstack/localstack:3.4")
            .WithPortBinding(4566, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(wait =>
            {
                return wait.ForPort(4566)
                           .ForPath("/")
                           .ForStatusCode(HttpStatusCode.OK);
            }))
            .Build();
        _container.StartAsync().GetAwaiter().GetResult();

        var serviceUrl = $"http://localhost:{_container.GetMappedPublicPort(4566)}";

        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "test");
        Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "test");
        Environment.SetEnvironmentVariable("AWS__AuthenticationRegion", "eu-west-1");
        Environment.SetEnvironmentVariable("AWS__ServiceURL", serviceUrl);

        var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
        _amazonSecretsManager = new ServiceCollection()
            .AddDefaultAWSOptions(config.GetAWSOptions())
            .AddAWSService<IAmazonSecretsManager>()
            .BuildServiceProvider()
            .GetRequiredService<IAmazonSecretsManager>();

        _fixture = new();
        _vault = _fixture.Create<string>();
        _secret = _fixture.Create<string>();
        _value = _fixture.Create<string>();

        // Ensure the secret is available before proceeding.
        CreateSecretAndWaitForAvailability();

        _mockLogger = new();
    }

    private void CreateSecretAndWaitForAvailability()
    {
        _amazonSecretsManager.CreateSecretAsync(new CreateSecretRequest
        {
            Name = _vault,
            SecretString = JsonSerializer.Serialize(new Dictionary<string, string> { [_secret] = _value })
        });

        var pipeline = new ResiliencePipelineBuilder<GetSecretValueResponse>()
            .AddRetry(new RetryStrategyOptions<GetSecretValueResponse>
            {
                ShouldHandle = new PredicateBuilder<GetSecretValueResponse>()
                    .Handle<ResourceNotFoundException>()
                    .HandleResult(static _ => _.HttpStatusCode != HttpStatusCode.OK),
                MaxRetryAttempts = 20,
                Delay = TimeSpan.FromSeconds(1) 
            })
            .Build();
        pipeline.Execute(() =>
        {
            System.Diagnostics.Trace.WriteLine("Waiting for secret to be available...");
            return _amazonSecretsManager.GetSecretValueAsync(new GetSecretValueRequest { SecretId = _vault, VersionStage = "AWSCURRENT" }).GetAwaiter().GetResult();
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _amazonSecretsManager?.Dispose();
                _container?.DisposeAsync();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
