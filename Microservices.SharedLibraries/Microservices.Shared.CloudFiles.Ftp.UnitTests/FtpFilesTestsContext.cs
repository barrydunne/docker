using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;
using Moq;

namespace Microservices.Shared.CloudFiles.Ftp.UnitTests;

internal class FtpFilesTestsContext
{
    private readonly Fixture _fixture;
    private readonly FtpFilesOptions _options;
    private readonly Mock<IOptions<FtpFilesOptions>> _mockOptions;
    private readonly MockAsyncFtpClient _mockAsyncFtpClient;
    private readonly MockLogger<FtpFiles> _mockLogger;

    internal FtpFiles Sut => new(_mockOptions.Object, _mockAsyncFtpClient.Object, _mockLogger.Object);

    internal FtpFilesTestsContext()
    {
        _fixture = new();
        _options = _fixture.Create<FtpFilesOptions>();
        _mockOptions = new(MockBehavior.Strict);
        _mockOptions.Setup(_ => _.Value).Returns(_options);
        _mockAsyncFtpClient = new();
        _mockLogger = new();
    }

    internal FtpFilesTestsContext WithDirectory(string container)
    {
        _mockAsyncFtpClient.AddDirectory($"{_options.BaseDir}/{container}");
        return this;
    }

    internal FtpFilesTestsContext WithFile(string container, string name, byte[] data)
    {
        _mockAsyncFtpClient.AddDirectory($"{_options.BaseDir}/{container}");
        _mockAsyncFtpClient.AddFile($"{_options.BaseDir}/{container}/{name}", data);
        return this;
    }

    internal FtpFilesTestsContext WithUploadException(InvalidDataException exception)
    {
        _mockAsyncFtpClient.WithUploadException(exception);
        return this;
    }

    internal FtpFilesTestsContext WithUploadFailure()
    {
        _mockAsyncFtpClient.WithUploadFailure();
        return this;
    }

    internal FtpFilesTestsContext WithDownloadFailure()
    {
        _mockAsyncFtpClient.WithDownloadFailure();
        return this;
    }

    internal FtpFilesTestsContext WithDownloadException(InvalidDataException exception)
    {
        _mockAsyncFtpClient.WithDownloadException(exception);
        return this;
    }

    internal FtpFilesTestsContext WithFileExistsException(InvalidDataException exception)
    {
        _mockAsyncFtpClient.WithFileExistsException(exception);
        return this;
    }

    internal FtpFilesTestsContext WithDeleteFileException(InvalidDataException exception)
    {
        _mockAsyncFtpClient.WithDeleteFileException(exception);
        return this;
    }

    internal FtpFilesTestsContext AssertHostSet()
    {
        Assert.That(_mockAsyncFtpClient.Host, Is.EqualTo(_options.Host));
        return this;
    }

    internal FtpFilesTestsContext AssertPortSet()
    {
        Assert.That(_mockAsyncFtpClient.Port, Is.EqualTo(_options.Port));
        return this;
    }

    internal FtpFilesTestsContext AssertDirectoryCreated(string container) => AssertDirectoryCreated(container, Times.Once());

    internal FtpFilesTestsContext AssertDirectoryNotCreated(string container) => AssertDirectoryCreated(container, Times.Never());

    internal FtpFilesTestsContext AssertDirectoryCreated(string container, Times times)
    {
        _mockAsyncFtpClient.Verify(_ => _.CreateDirectory($"{_options.BaseDir}/{container}", It.IsAny<CancellationToken>()), times);
        return this;
    }

    internal FtpFilesTestsContext AssertFileUploaded(string container, string name, byte[] data)
    {
        _mockAsyncFtpClient.Files.TryGetValue($"{_options.BaseDir}/{container}/{name}", out var remote);
        Assert.That(remote?.SequenceEqual(data), Is.True);
        return this;
    }

    internal FtpFilesTestsContext AssertFileDeleted(string container, string name)
    {
        _mockAsyncFtpClient.Verify(_ => _.DeleteFile($"{_options.BaseDir}/{container}/{name}", It.IsAny<CancellationToken>()));
        return this;
    }
}
