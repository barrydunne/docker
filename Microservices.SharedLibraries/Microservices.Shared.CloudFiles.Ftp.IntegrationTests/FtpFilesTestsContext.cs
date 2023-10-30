using FluentFTP;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;
using Moq;

namespace Microservices.Shared.CloudFiles.Ftp.IntegrationTests
{
    internal class FtpFilesTestsContext : IDisposable
    {
        private readonly FtpFilesOptions _options;
        private readonly Mock<IOptions<FtpFilesOptions>> _mockOptions;
        private readonly AsyncFtpClient _asyncFtpClient;
        private readonly MockLogger<FtpFiles> _mockLogger;
        private bool _disposedValue;

        internal FtpFiles Sut => new(_mockOptions.Object, _asyncFtpClient, _mockLogger.Object);

        internal FtpFilesTestsContext()
        {
            _options = new() { Host = "localhost", Port = 10021, BaseDir = "/files" };
            _mockOptions = new(MockBehavior.Strict);
            _mockOptions.Setup(_ => _.Value).Returns(_options);
            _asyncFtpClient = new();
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
}
