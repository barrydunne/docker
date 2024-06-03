using AspNet.KickStarter;
using AutoFixture;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Microservices.Shared.CloudEmail.Smtp.IntegrationTests;

internal class SmtpEmailTestsContext : IDisposable
{
    private readonly Fixture _fixture;
    private readonly SmtpEmailOptions _options;
    private readonly IOptions<SmtpEmailOptions> _mockOptions;
    private readonly SmtpClientAdapter _smtpClient;
    private readonly ITraceActivity _mockTraceActivity;
    private readonly MockLogger<SmtpEmail> _mockLogger;

    private bool _disposedValue;

    internal SmtpEmail Sut { get; }

    internal SmtpEmailTestsContext()
    {
        _fixture = new();
        _options = new() { Host = "localhost", Port = 10025 };
        _mockOptions = Substitute.For<IOptions<SmtpEmailOptions>>();
        _mockOptions
            .Value
            .Returns(callInfo => _options);
        _smtpClient = new();
        _mockTraceActivity = Substitute.For<ITraceActivity>();
        _mockLogger = new();
        _disposedValue = false;
         
        Sut = new(_mockOptions, _smtpClient, _mockTraceActivity, _mockLogger);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _smtpClient.Dispose();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
