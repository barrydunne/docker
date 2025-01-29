using FluentFTP;
using Microservices.Shared.CloudFiles;

namespace Microservices.Shared.Mocks;

public class MockCloudFiles : ICloudFiles
{
    private readonly IProgress<FtpProgress> _mockProgress;
    private readonly MockAsyncFtpClient _client;

    public MockCloudFiles()
    {
        _mockProgress = Substitute.For<IProgress<FtpProgress>>();
        _client = new();
    }

    public async Task<bool> DeleteFileAsync(string container, string name, CancellationToken cancellationToken = default)
    {
        await _client.DeleteFile(GetPath(container, name));
        return true;
    }

    public async Task<bool> DownloadFileAsync(string container, string name, Stream content, CancellationToken cancellationToken = default)
    {
        await _client.DownloadStream(content, GetPath(container, name), 0, _mockProgress, cancellationToken, 0);
        return true;
    }

    public async Task<bool> FileExistsAsync(string container, string name, CancellationToken cancellationToken = default)
        => await _client.FileExists(GetPath(container, name), cancellationToken);

    public async Task<bool> UploadFileAsync(string container, string name, Stream content, CancellationToken cancellationToken = default)
    {
        await _client.UploadStream(content, GetPath(container, name), FtpRemoteExists.NoCheck, false, _mockProgress, cancellationToken);
        return true;
    }

    public void AddFile(string container, string name, Stream content)
        => _client.UploadStream(content, GetPath(container, name), FtpRemoteExists.NoCheck, true, _mockProgress, CancellationToken.None).Wait();

    private static string GetPath(string container, string name) => $"{container}/{name}";
}
