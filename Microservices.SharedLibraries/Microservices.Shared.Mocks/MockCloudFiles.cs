using FluentFTP;
using Microservices.Shared.CloudFiles;
using Moq;

namespace Microservices.Shared.Mocks;

public class MockCloudFiles : Mock<ICloudFiles>
{
    private readonly Mock<IProgress<FtpProgress>> _mockProgress;
    private readonly MockAsyncFtpClient _client;

    public MockCloudFiles() : base(MockBehavior.Strict)
    {
        _mockProgress = new();
        _client = new();

        Setup(_ => _.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback((string container, string name, Stream content, CancellationToken token) => _client.Object.UploadStream(content, GetPath(container, name), FtpRemoteExists.NoCheck, false, _mockProgress.Object, token))
            .ReturnsAsync(() => true);

        Setup(_ => _.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback((string container, string name, Stream content, CancellationToken token) => _client.Object.DownloadStream(content, GetPath(container, name), 0, _mockProgress.Object, token, 0))
            .ReturnsAsync(() => true);

        Setup(_ => _.FileExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns((string container, string name, CancellationToken token) => _client.Object.FileExists(GetPath(container, name), token));

        Setup(_ => _.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback((string container, string name, CancellationToken _) => _client.Object.DeleteFile(GetPath(container, name)))
            .ReturnsAsync(() => true);
    }

    public void AddFile(string container, string name, Stream content)
        => _client.Object.UploadStream(content, GetPath(container, name), FtpRemoteExists.NoCheck, true, _mockProgress.Object, CancellationToken.None).Wait();

    private static string GetPath(string container, string name) => $"{container}/{name}";
}
