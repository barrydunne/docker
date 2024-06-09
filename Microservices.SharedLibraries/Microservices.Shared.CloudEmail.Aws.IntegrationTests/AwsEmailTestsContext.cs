using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using AutoFixture;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net;
using System.Net.Mail;

namespace Microservices.Shared.CloudEmail.Aws.IntegrationTests;

internal class AwsEmailTestsContext : IDisposable
{
    private readonly IContainer _container;
    private readonly Fixture _fixture;
    private readonly AwsEmailOptions _options;
    private readonly IOptions<AwsEmailOptions> _mockOptions;
    private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;
    private readonly MockLogger<AwsEmail> _mockLogger;

    private bool _disposedValue;

    internal AwsEmail Sut { get; }

    internal AwsEmailTestsContext()
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

        _fixture = new();

        _options = new() { From = _fixture.Create<MailAddress>().Address };
        _mockOptions = Substitute.For<IOptions<AwsEmailOptions>>();
        _mockOptions.Value.Returns(_options);

        var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
        _amazonSimpleEmailService = new ServiceCollection()
            .AddSingleton(_mockOptions)
            .AddDefaultAWSOptions(config.GetAWSOptions())
            .AddAWSService<IAmazonSimpleEmailService>()
            .BuildServiceProvider()
            .GetRequiredService<IAmazonSimpleEmailService>();

        _amazonSimpleEmailService.VerifyEmailIdentityAsync(new VerifyEmailIdentityRequest() { EmailAddress = _options.From }).GetAwaiter().GetResult();

        _mockLogger = new();
         
        Sut = new(_mockOptions, _amazonSimpleEmailService, _mockLogger);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _container.DisposeAsync().GetAwaiter().GetResult();
                _amazonSimpleEmailService.Dispose();
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
