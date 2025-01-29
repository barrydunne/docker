using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;

namespace Microservices.Shared.CloudFiles.Ftp.UnitTests;

internal class FtpFilesTestsContext
{
    private readonly Fixture _fixture;
    private readonly FtpFilesOptions _options;
    private readonly IOptions<FtpFilesOptions> _mockOptions;
    private readonly MockAsyncFtpClient _mockAsyncFtpClient;
    private readonly MockLogger<FtpFiles> _mockLogger;

    internal FtpFiles Sut => new(_mockOptions, _mockAsyncFtpClient, _mockLogger);

    internal FtpFilesTestsContext()
    {
        _fixture = new();
        _options = _fixture.Create<FtpFilesOptions>();
        _mockOptions = Substitute.For<IOptions<FtpFilesOptions>>();
        _mockOptions
            .Value
            .Returns(callInfo => _options);
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
        _mockAsyncFtpClient.Host.ShouldBe(_options.Host);
        return this;
    }

    internal FtpFilesTestsContext AssertPortSet()
    {
        _mockAsyncFtpClient.Port.ShouldBe(_options.Port);
        return this;
    }

    internal FtpFilesTestsContext AssertDirectoryCreated(string container) => AssertDirectoryCreated(container, 1);

    internal FtpFilesTestsContext AssertDirectoryNotCreated(string container) => AssertDirectoryCreated(container, 0);

    internal FtpFilesTestsContext AssertDirectoryCreated(string container, int times)
    {
        _mockAsyncFtpClient.AssertDirectoryCreated($"{_options.BaseDir}/{container}", times);
        return this;
    }

    internal FtpFilesTestsContext AssertFileUploaded(string container, string name, byte[] data)
    {
        _mockAsyncFtpClient.Files.TryGetValue($"{_options.BaseDir}/{container}/{name}", out var remote);
        remote.ShouldBe(data);
        return this;
    }

    internal FtpFilesTestsContext AssertFileDeleted(string container, string name)
    {
        _mockAsyncFtpClient.AssertFileDeleted($"{_options.BaseDir}/{container}/{name}", 1);
        return this;
    }
}
