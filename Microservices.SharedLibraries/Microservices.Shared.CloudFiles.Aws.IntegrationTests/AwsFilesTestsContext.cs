using Amazon.S3;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net;

namespace Microservices.Shared.CloudFiles.Aws.IntegrationTests;

internal class AwsFilesTestsContext : IDisposable
{
    private readonly IContainer _container;
    private readonly AwsFilesOptions _options;
    private readonly IOptions<AwsFilesOptions> _mockOptions;
    private readonly IAmazonS3 _amazonS3;
    private readonly MockLogger<AwsFiles> _mockLogger;
    private bool _disposedValue;

    internal AwsFiles Sut => new(_mockOptions, _amazonS3, _mockLogger);

    internal AwsFilesTestsContext()
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

        _options = new AwsFilesOptions { Region = "eu-west-1" };
        _mockOptions = Substitute.For<IOptions<AwsFilesOptions>>();
        _mockOptions.Value.Returns(_options);

        var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
        _amazonS3 = new ServiceCollection()
            .AddSingleton(_mockOptions)
            .AddDefaultAWSOptions(config.GetAWSOptions())
            .AddAWSService<IAmazonS3>()
            .BuildServiceProvider()
            .GetRequiredService<IAmazonS3>();

        // Use path style addressing for localstack.
        //   http://s3.eu-west-1.amazonaws.com/bucketname
        // instead of
        //   http://bucketname.s3.eu-west-1.amazonaws.com
        (_amazonS3.Config as AmazonS3Config)!.ForcePathStyle = true;

        _mockLogger = new();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _amazonS3?.Dispose();
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
