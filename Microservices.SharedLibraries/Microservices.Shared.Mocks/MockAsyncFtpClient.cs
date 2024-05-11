using FluentFTP;
using FluentFTP.Model.Functions;
using FluentFTP.Rules;
using FluentFTP.Servers;
using System.Collections.Concurrent;
using System.Net;
using System.Security.Authentication;
using System.Text;

namespace Microservices.Shared.Mocks;

public class MockAsyncFtpClient : IAsyncFtpClient
{
    private readonly ConcurrentDictionary<string, byte> _directories; // Use ConcurrentDictionary as there is no ConcurrentHashSet
    private readonly ConcurrentDictionary<string, byte[]> _files;
    private readonly ConcurrentBag<string> _directoriesCreated;
    private readonly ConcurrentBag<string> _deletedFiles;

    private Exception? _uploadException;
    private bool _uploadFailure;
    private Exception? _downloadException;
    private bool _downloadFailure;
    private Exception? _fileExistsException;
    private Exception? _deleteFileException;

    public FtpConfig Config { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IFtpLogger Logger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool IsDisposed => throw new NotImplementedException();

    public bool IsConnected => throw new NotImplementedException();

    public IReadOnlyCollection<string> Directories => _directories.Keys.ToArray();
    public IReadOnlyDictionary<string, byte[]> Files => _files;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public NetworkCredential Credentials { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public List<FtpCapability> Capabilities => throw new NotImplementedException();

    public FtpHashAlgorithm HashAlgorithms => throw new NotImplementedException();

    public string SystemType => throw new NotImplementedException();

    public FtpServer ServerType => throw new NotImplementedException();

    public FtpBaseServer ServerHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public FtpOperatingSystem ServerOS => throw new NotImplementedException();

    public string ConnectionType => throw new NotImplementedException();

    public FtpReply LastReply => throw new NotImplementedException();

    public List<FtpReply> LastReplies { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Encoding Encoding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Action<FtpTraceLevel, string> LegacyLogger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public FtpClientState Status => throw new NotImplementedException();

    public FtpIpVersion? InternetProtocol => throw new NotImplementedException();

    public bool IsAuthenticated => throw new NotImplementedException();

    public SslProtocols SslProtocolActive => throw new NotImplementedException();

    public bool IsEncrypted => throw new NotImplementedException();

    public bool ValidateCertificateHandlerExists => throw new NotImplementedException();

    public bool RecursiveList => throw new NotImplementedException();

    public IPEndPoint SocketLocalEndPoint => throw new NotImplementedException();

    public IPEndPoint SocketRemoteEndPoint => throw new NotImplementedException();

#pragma warning disable CS0414 // The field 'MockAsyncFtpClient.ValidateCertificate' is assigned but its value is never used
    public event FtpSslValidation ValidateCertificate;
#pragma warning restore CS0414 // The field 'MockAsyncFtpClient.ValidateCertificate' is assigned but its value is never used

    public MockAsyncFtpClient()
    {
        _directories = new();
        _files = new();
        _directoriesCreated = new();
        _deletedFiles = new();
        _uploadException = null;
        _uploadFailure = false;
        _fileExistsException = null;
        _deleteFileException = null;

        ValidateCertificate = null!;
    }

    public void AddDirectory(string directory) => _directories[directory] = 1;
    public void AddFile(string path, byte[] data) => _files[path] = data;
    public void WithUploadException(Exception? exception) => _uploadException = exception;
    public void WithUploadFailure() => _uploadFailure = true;
    public void WithDownloadException(Exception? exception) => _downloadException = exception;
    public void WithDownloadFailure() => _downloadFailure = true;
    public void WithFileExistsException(Exception? exception) => _fileExistsException = exception;
    public void WithDeleteFileException(InvalidDataException exception) => _deleteFileException = exception;

    public void AssertDirectoryCreated(string path, int count)
    {
        var actualCount = _directoriesCreated.Count(_ => _ == path);
        if (actualCount != count)
            throw new KeyNotFoundException($"Expected directory create count to be {count} but was {actualCount} : \"{path}\"");
    }

    public void AssertFileDeleted(string path, int count)
    {
        var actualCount = _deletedFiles.Count(_ => _ == path);
        if (actualCount != count)
            throw new KeyNotFoundException($"Expected file delete count to be {count} but was {actualCount} : \"{path}\"");
    }

    private FtpStatus UploadStream(Stream content, string path, bool createDir, IProgress<FtpProgress> progress)
    {
        if (_uploadFailure)
            return FtpStatus.Failed;
        if (_uploadException is not null)
            throw _uploadException;

        var dir = Path.GetDirectoryName(path)!.Replace('\\', '/');
        if (!(createDir || _directories.ContainsKey(dir)))
            return FtpStatus.Failed;
        progress.Report(new FtpProgress(0, 0, 0, TimeSpan.FromSeconds(1), string.Empty, path, null));
        using var mem = new MemoryStream();
        content.CopyTo(mem);
        var data = mem.ToArray();
        _files[path] = data;
        progress.Report(new FtpProgress(100, data.Length, 1000, TimeSpan.Zero, string.Empty, path, null));
        return FtpStatus.Success;
    }

    private bool DownloadStream(Stream content, string path, IProgress<FtpProgress> progress)
    {
        if (_downloadFailure)
            return false;
        if (_downloadException is not null)
            throw _downloadException;

        if (!_files.ContainsKey(path))
            return false;
        progress.Report(new FtpProgress(0, 0, 0, TimeSpan.FromSeconds(1), string.Empty, path, null));
        var data = _files[path];
        content.Write(data);
        progress.Report(new FtpProgress(100, data.Length, 1000, TimeSpan.Zero, string.Empty, path, null));
        return true;
    }

    private bool FileExists(string path)
    {
        if (_fileExistsException is not null)
            throw _fileExistsException;
        return _files.ContainsKey(path);
    }

    private void DeleteFile(string path)
    {
        _deletedFiles.Add(path);
        if (_deleteFileException is not null)
            throw _deleteFileException;
        _files.TryRemove(path, out var _);
    }

    public Task<FtpProfile> AutoConnect(CancellationToken token = default) => throw new NotImplementedException();
    public Task<List<FtpProfile>> AutoDetect(FtpAutoDetectConfig config, CancellationToken token = default) => throw new NotImplementedException();
    public Task<List<FtpProfile>> AutoDetect(bool firstOnly, bool cloneConnection = true, CancellationToken token = default) => throw new NotImplementedException();
    public Task Chmod(string path, int permissions, CancellationToken token = default) => throw new NotImplementedException();
    public Task Chmod(string path, FtpPermission owner, FtpPermission group, FtpPermission other, CancellationToken token = default) => throw new NotImplementedException();
    public Task<FtpCompareResult> CompareFile(string localPath, string remotePath, FtpCompareOption options = FtpCompareOption.Auto, CancellationToken token = default) => throw new NotImplementedException();
    public Task Connect(CancellationToken token = default)
        => Task.CompletedTask;
    public Task Connect(FtpProfile profile, CancellationToken token = default) => throw new NotImplementedException();
    public Task Connect(bool reConnect, CancellationToken token = default) => throw new NotImplementedException();
    public Task<bool> CreateDirectory(string path, bool force, CancellationToken token = default) => throw new NotImplementedException();
    public Task<bool> CreateDirectory(string path, CancellationToken token = default)
    {
        _directories[path] = 1;
        _directoriesCreated.Add(path);
        return Task.FromResult(true);
    }
    public Task DeleteDirectory(string path, CancellationToken token = default) => throw new NotImplementedException();
    public Task DeleteDirectory(string path, FtpListOption options, CancellationToken token = default) => throw new NotImplementedException();
    public Task DeleteFile(string path, CancellationToken token = default)
    {
        DeleteFile(path);
        return Task.CompletedTask;
    }
    public Task<bool> DirectoryExists(string path, CancellationToken token = default)
        => Task.FromResult(_directories.ContainsKey(path));
    public Task DisableUTF8(CancellationToken token = default) => throw new NotImplementedException();
    public Task Disconnect(CancellationToken token = default)
        => Task.CompletedTask;
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
    public Task<byte[]> DownloadBytes(string remotePath, long restartPosition = 0, IProgress<FtpProgress> progress = null!, CancellationToken token = default, long stopPosition = 0) => throw new NotImplementedException();
    public Task<byte[]> DownloadBytes(string remotePath, CancellationToken token = default) => throw new NotImplementedException();
    public Task<List<FtpResult>> DownloadDirectory(string localFolder, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update, FtpLocalExists existsMode = FtpLocalExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null!, IProgress<FtpProgress> progress = null!, CancellationToken token = default) => throw new NotImplementedException();
    public Task<FtpStatus> DownloadFile(string localPath, string remotePath, FtpLocalExists existsMode = FtpLocalExists.Overwrite, FtpVerify verifyOptions = FtpVerify.None, IProgress<FtpProgress> progress = null!, CancellationToken token = default) => throw new NotImplementedException();
    public Task<List<FtpResult>> DownloadFiles(string localDir, IEnumerable<string> remotePaths, FtpLocalExists existsMode = FtpLocalExists.Overwrite, FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, CancellationToken token = default, IProgress<FtpProgress> progress = null!, List<FtpRule> rules = null!) => throw new NotImplementedException();
    public Task<bool> DownloadStream(Stream outStream, string remotePath, long restartPosition = 0, IProgress<FtpProgress> progress = null!, CancellationToken token = default, long stopPosition = 0)
        => Task.FromResult(DownloadStream(outStream, remotePath, progress));
    public Task<byte[]> DownloadUriBytes(string uri, IProgress<FtpProgress> progress = null!, CancellationToken token = default) => throw new NotImplementedException();
    public Task EmptyDirectory(string path, CancellationToken token = default) => throw new NotImplementedException();
    public Task EmptyDirectory(string path, FtpListOption options, CancellationToken token = default) => throw new NotImplementedException();
    public Task<FtpReply> Execute(string command, CancellationToken token = default) => throw new NotImplementedException();
    public Task<List<string>> ExecuteDownloadText(string command, CancellationToken token = default) => throw new NotImplementedException();
    public Task<bool> FileExists(string path, CancellationToken token = default)
        => Task.FromResult(FileExists(path));
    public Task<FtpHash> GetChecksum(string path, FtpHashAlgorithm algorithm = FtpHashAlgorithm.NONE, CancellationToken token = default) => throw new NotImplementedException();
    public Task<int> GetChmod(string path, CancellationToken token = default) => throw new NotImplementedException();
    public Task<FtpListItem> GetFilePermissions(string path, CancellationToken token = default) => throw new NotImplementedException();
    public Task<long> GetFileSize(string path, long defaultValue = -1, CancellationToken token = default) => throw new NotImplementedException();
    public Task<FtpListItem[]> GetListing(string path, FtpListOption options, CancellationToken token = default) => throw new NotImplementedException();
    public Task<FtpListItem[]> GetListing(string path, CancellationToken token = default) => throw new NotImplementedException();
    public Task<FtpListItem[]> GetListing(CancellationToken token = default) => throw new NotImplementedException();
    public IAsyncEnumerable<FtpListItem> GetListingEnumerable(string path, FtpListOption options, CancellationToken token = default, CancellationToken enumToken = default) => throw new NotImplementedException();
    public IAsyncEnumerable<FtpListItem> GetListingEnumerable(string path, CancellationToken token = default, CancellationToken enumToken = default) => throw new NotImplementedException();
    public IAsyncEnumerable<FtpListItem> GetListingEnumerable(CancellationToken token = default, CancellationToken enumToken = default) => throw new NotImplementedException();
    public Task<DateTime> GetModifiedTime(string path, CancellationToken token = default) => throw new NotImplementedException();
    public Task<string[]> GetNameListing(string path, CancellationToken token = default) => throw new NotImplementedException();
    public Task<string[]> GetNameListing(CancellationToken token = default) => throw new NotImplementedException();
    public Task<FtpListItem> GetObjectInfo(string path, bool dateModified = false, CancellationToken token = default) => throw new NotImplementedException();
    public Task<FtpReply> GetReply(CancellationToken token = default) => throw new NotImplementedException();
    public Task<string> GetWorkingDirectory(CancellationToken token = default) => throw new NotImplementedException();
    public bool HasFeature(FtpCapability cap) => throw new NotImplementedException();
    public Task<bool> MoveDirectory(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, CancellationToken token = default) => throw new NotImplementedException();
    public Task<bool> MoveFile(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, CancellationToken token = default) => throw new NotImplementedException();
    public Task<Stream> OpenAppend(string path, FtpDataType type = FtpDataType.Binary, bool checkIfFileExists = true, CancellationToken token = default) => throw new NotImplementedException();
    public Task<Stream> OpenAppend(string path, FtpDataType type, long fileLen, CancellationToken token = default) => throw new NotImplementedException();
    public Task<Stream> OpenRead(string path, FtpDataType type = FtpDataType.Binary, long restart = 0, bool checkIfFileExists = true, CancellationToken token = default) => throw new NotImplementedException();
    public Task<Stream> OpenRead(string path, FtpDataType type, long restart, long fileLen, CancellationToken token = default) => throw new NotImplementedException();
    public Task<Stream> OpenWrite(string path, FtpDataType type = FtpDataType.Binary, bool checkIfFileExists = true, CancellationToken token = default) => throw new NotImplementedException();
    public Task<Stream> OpenWrite(string path, FtpDataType type, long fileLen, CancellationToken token = default) => throw new NotImplementedException();
    public Task Rename(string path, string dest, CancellationToken token = default) => throw new NotImplementedException();
    public Task SetFilePermissions(string path, int permissions, CancellationToken token = default) => throw new NotImplementedException();
    public Task SetFilePermissions(string path, FtpPermission owner, FtpPermission group, FtpPermission other, CancellationToken token = default) => throw new NotImplementedException();
    public Task SetModifiedTime(string path, DateTime date, CancellationToken token = default) => throw new NotImplementedException();
    public Task SetWorkingDirectory(string path, CancellationToken token = default) => throw new NotImplementedException();
    public Task<FtpStatus> UploadBytes(byte[] fileData, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, IProgress<FtpProgress> progress = null!, CancellationToken token = default) => throw new NotImplementedException();
    public Task<List<FtpResult>> UploadDirectory(string localFolder, string remoteFolder, FtpFolderSyncMode mode = FtpFolderSyncMode.Update, FtpRemoteExists existsMode = FtpRemoteExists.Skip, FtpVerify verifyOptions = FtpVerify.None, List<FtpRule> rules = null!, IProgress<FtpProgress> progress = null!, CancellationToken token = default) => throw new NotImplementedException();
    public Task<FtpStatus> UploadFile(string localPath, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, FtpVerify verifyOptions = FtpVerify.None, IProgress<FtpProgress> progress = null!, CancellationToken token = default) => throw new NotImplementedException();
    public Task<List<FtpResult>> UploadFiles(IEnumerable<string> localPaths, string remoteDir, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = true, FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, CancellationToken token = default, IProgress<FtpProgress> progress = null!, List<FtpRule> rules = null!) => throw new NotImplementedException();
    public Task<List<FtpResult>> UploadFiles(IEnumerable<FileInfo> localFiles, string remoteDir, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = true, FtpVerify verifyOptions = FtpVerify.None, FtpError errorHandling = FtpError.None, CancellationToken token = default, IProgress<FtpProgress> progress = null!, List<FtpRule> rules = null!) => throw new NotImplementedException();
    public Task<FtpStatus> UploadStream(Stream fileStream, string remotePath, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, bool createRemoteDir = false, IProgress<FtpProgress> progress = null!, CancellationToken token = default)
        => Task.FromResult(UploadStream(fileStream, remotePath, createRemoteDir, progress));
}
