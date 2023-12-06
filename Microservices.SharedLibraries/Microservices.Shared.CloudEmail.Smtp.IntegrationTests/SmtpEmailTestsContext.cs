using AutoFixture;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;
using Moq;

namespace Microservices.Shared.CloudEmail.Smtp.IntegrationTests;

internal class SmtpEmailTestsContext : IDisposable
{
    private readonly Fixture _fixture;
    private readonly SmtpEmailOptions _options;
    private readonly Mock<IOptions<SmtpEmailOptions>> _mockOptions;
    private readonly SmtpClientAdapter _smtpClient;
    private readonly MockLogger<SmtpEmail> _mockLogger;

    private bool _disposedValue;

    internal SmtpEmail Sut { get; }

    internal SmtpEmailTestsContext()
    {
        _fixture = new();
        _options = new() { Host = "localhost", Port = 10025 };
        _mockOptions = new(MockBehavior.Strict);
        _mockOptions.Setup(_ => _.Value).Returns(_options);
        _smtpClient = new();
        _mockLogger = new();
        _disposedValue = false;
         
        Sut = new(_mockOptions.Object, _smtpClient, _mockLogger.Object);
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
