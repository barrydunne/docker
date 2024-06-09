using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microservices.Shared.Mocks;
using Microservices.Shared.RestSharpFactory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net;

namespace Microservices.Shared.CloudSecrets.SecretsManager.IntegrationTests;

internal class SecretsManagerSecretsTestsContext : IDisposable
{
    private readonly IContainer _container;
    private readonly IOptions<SecretsManagerOptions> _mockOptions;
    private readonly MockLogger<SecretsManagerSecrets> _mockLogger;

    private bool _disposedValue;

    internal SecretsManagerSecrets Sut => new(_mockOptions, new RestSharpClientFactory(), _mockLogger);

    internal SecretsManagerSecretsTestsContext()
    {
        // Run a container with SMTP support. Bind port 1025 to random local port, and wait for HTTP site to be available.
        _container = new ContainerBuilder()
            .WithImage("ghcr.io/aaronpowell/httpstatus:c82331cbde67f430da66a84a758d94ba5afd7620")
            .WithPortBinding(80, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(wait =>
            {
                return wait.ForPort(80)
                           .ForPath("/200")
                           .ForStatusCode(HttpStatusCode.OK);
            }))
            .Build();
        _container.StartAsync().GetAwaiter().GetResult();

        _mockOptions = Substitute.For<IOptions<SecretsManagerOptions>>();
        _mockOptions
            .Value
            .Returns(callInfo => new SecretsManagerOptions { BaseUrl = "http://localhost:10083" });

        _mockLogger = new();
    }

    internal SecretsManagerSecretsTestsContext WithForbiddenResponse()
    {
        _mockOptions
            .Value
            .Returns(callInfo => new SecretsManagerOptions { BaseUrl = $"http://localhost:{_container.GetMappedPublicPort(80)}/403" });
        return this;
    }

    internal SecretsManagerSecretsTestsContext WithProblemResponse()
    {
        _mockOptions
            .Value
            .Returns(callInfo => new SecretsManagerOptions { BaseUrl = $"http://localhost:{_container.GetMappedPublicPort(80)}/505" });
        return this;
    }

    internal SecretsManagerSecretsTestsContext WithUnknownHost()
    {
        _mockOptions
            .Value
            .Returns(callInfo => new SecretsManagerOptions { BaseUrl = "http://-0" });
        return this;
    }

    internal SecretsManagerSecretsTestsContext AssertWarningLogged(string message)
    {
        Assert.That(_mockLogger.Messages, Does.Contain($"[{LogLevel.Warning}] {message}"));
        return this;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _container.DisposeAsync().GetAwaiter().GetResult();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
