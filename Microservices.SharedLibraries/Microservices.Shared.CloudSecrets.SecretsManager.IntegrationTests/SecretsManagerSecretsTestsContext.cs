using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microservices.Shared.Mocks;
using Microservices.Shared.RestSharpFactory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Reflection;

namespace Microservices.Shared.CloudSecrets.SecretsManager.IntegrationTests;

internal class SecretsManagerSecretsTestsContext : IDisposable
{
    private readonly IContainer _containerApi;
    private readonly IContainer _containerStatusCodes;
    private readonly IOptions<SecretsManagerOptions> _mockOptions;
    private readonly MockRestSharpResiliencePipeline _mockRestSharpResiliencePipeline;
    private readonly MockLogger<SecretsManagerSecrets> _mockLogger;

    private bool _disposedValue;

    internal SecretsManagerSecrets Sut => new(_mockOptions, new RestSharpClientFactory(), _mockRestSharpResiliencePipeline, _mockLogger);

    internal SecretsManagerSecretsTestsContext()
    {
        // Run a container with simulated API responses.
        var conf = GetNginxConf();
        _containerApi = new ContainerBuilder()
            .WithImage("nginx:1.25.3")
            .WithName($"SecretsManagerTests.Nginx_{Guid.NewGuid():N}")
            .WithResourceMapping(conf, "/etc/nginx/conf.d/default.conf")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
            .WithPortBinding(80, true)
            .Build();
        _containerApi.StartAsync().GetAwaiter().GetResult();

        // Run a container with HTTP status code support.
        _containerStatusCodes = new ContainerBuilder()
            .WithImage("ghcr.io/aaronpowell/httpstatus:c82331cbde67f430da66a84a758d94ba5afd7620")
            .WithName($"SecretsManagerTests.HttpStatus_{Guid.NewGuid():N}")
            .WithPortBinding(80, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(wait =>
            {
                return wait.ForPort(80)
                           .ForPath("/200")
                           .ForStatusCode(HttpStatusCode.OK);
            }))
            .Build();
        _containerStatusCodes.StartAsync().GetAwaiter().GetResult();

        _mockOptions = Substitute.For<IOptions<SecretsManagerOptions>>();
        _mockOptions
            .Value
            .Returns(callInfo => new SecretsManagerOptions { BaseUrl = $"http://localhost:{_containerApi.GetMappedPublicPort(80)}" });

        _mockRestSharpResiliencePipeline = new();
        _mockLogger = new();
    }

    private byte[] GetNginxConf()
    {
        using var mem = new MemoryStream();
        using var source = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microservices.Shared.CloudSecrets.SecretsManager.IntegrationTests.nginx.conf");
        source!.CopyTo(mem);
        return mem.ToArray();
    }

    internal SecretsManagerSecretsTestsContext WithForbiddenResponse()
    {
        _mockOptions
            .Value
            .Returns(callInfo => new SecretsManagerOptions { BaseUrl = $"http://localhost:{_containerStatusCodes.GetMappedPublicPort(80)}/403" });
        return this;
    }

    internal SecretsManagerSecretsTestsContext WithProblemResponse()
    {
        _mockOptions
            .Value
            .Returns(callInfo => new SecretsManagerOptions { BaseUrl = $"http://localhost:{_containerStatusCodes.GetMappedPublicPort(80)}/505" });
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
        _mockLogger.Messages.ShouldContain($"[{LogLevel.Warning}] {message}");
        return this;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _containerApi.DisposeAsync().GetAwaiter().GetResult();
                _containerStatusCodes.DisposeAsync().GetAwaiter().GetResult();
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
