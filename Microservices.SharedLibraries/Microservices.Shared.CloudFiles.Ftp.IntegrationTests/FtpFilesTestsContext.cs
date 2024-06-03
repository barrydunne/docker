using AspNet.KickStarter;
using FluentFTP;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Microservices.Shared.CloudFiles.Ftp.IntegrationTests;

internal class FtpFilesTestsContext : IDisposable
{
    private readonly FtpFilesOptions _options;
    private readonly IOptions<FtpFilesOptions> _mockOptions;
    private readonly AsyncFtpClient _asyncFtpClient;
    private readonly ITraceActivity _mockTraceActivity;
    private readonly MockLogger<FtpFiles> _mockLogger;
    private bool _disposedValue;

    internal FtpFiles Sut => new(_mockOptions, _asyncFtpClient, _mockTraceActivity, _mockLogger);

    internal FtpFilesTestsContext()
    {
        _options = new() { Host = "localhost", Port = 10021, BaseDir = "/files" };
        _mockOptions = Substitute.For<IOptions<FtpFilesOptions>>();
        _mockOptions.Value.Returns(_options);
        _asyncFtpClient = new();
        _mockTraceActivity = Substitute.For<ITraceActivity>();
        _mockLogger = new();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _asyncFtpClient?.Dispose();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
