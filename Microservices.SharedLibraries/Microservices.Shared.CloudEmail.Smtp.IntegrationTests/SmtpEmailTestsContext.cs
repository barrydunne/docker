using AutoFixture;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net;

namespace Microservices.Shared.CloudEmail.Smtp.IntegrationTests;

internal class SmtpEmailTestsContext : IDisposable
{
    private readonly IContainer _container;
    private readonly Fixture _fixture;
    private readonly SmtpEmailOptions _options;
    private readonly IOptions<SmtpEmailOptions> _mockOptions;
    private readonly SmtpClientAdapter _smtpClient;
    private readonly MockLogger<SmtpEmail> _mockLogger;

    private bool _disposedValue;

    internal SmtpEmail Sut { get; }

    internal SmtpEmailTestsContext()
    {
        // Run a container with SMTP support. Bind port 1025 to random local port, and wait for HTTP site to be available.
        _container = new ContainerBuilder()
            .WithImage("dockage/mailcatcher:0.9.0")
            .WithPortBinding(1025, true)
            .WithPortBinding(1080, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(wait =>
            {
                return wait.ForPort(1080)
                           .ForPath("/messages")
                           .ForStatusCode(HttpStatusCode.OK);
            }))
            .Build();
        _container.StartAsync().GetAwaiter().GetResult();

        _fixture = new();
        _options = new() { Host = "localhost", Port = _container.GetMappedPublicPort(1025) };
        _mockOptions = Substitute.For<IOptions<SmtpEmailOptions>>();
        _mockOptions
            .Value
            .Returns(callInfo => _options);
        _smtpClient = new();
        _mockLogger = new();
         
        Sut = new(_mockOptions, _smtpClient, _mockLogger);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _container.DisposeAsync().GetAwaiter().GetResult();
                _smtpClient.Dispose();
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
