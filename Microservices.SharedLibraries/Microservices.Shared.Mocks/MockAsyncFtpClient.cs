using FluentFTP;
using Moq;
using System.Collections.Concurrent;

namespace Microservices.Shared.Mocks
{
    public class MockAsyncFtpClient : Mock<IAsyncFtpClient>
    {
        private readonly ConcurrentDictionary<string, byte> _directories; // Use ConcurrentDictionary as there is no ConcurrentHashSet
        private readonly ConcurrentDictionary<string, byte[]> _files;

        private Exception? _uploadException;
        private bool _uploadFailure;
        private Exception? _downloadException;
        private bool _downloadFailure;
        private Exception? _fileExistsException;
        private Exception? _deleteFileException;

        public string? Host { get; private set; }
        public int? Port { get; private set; }
        public IReadOnlyCollection<string> Directories => _directories.Keys.ToArray();
        public IReadOnlyDictionary<string, byte[]> Files => _files;

        public MockAsyncFtpClient() : base(MockBehavior.Strict)
        {
            _directories = new();
            _files = new();
            _uploadException = null;
            _uploadFailure = false;
            _fileExistsException = null;
            _deleteFileException = null;

            SetupSet(_ => _.Host = It.IsAny<string>())
                .Callback((string host) => Host = host);
            
            SetupSet(_ => _.Port = It.IsAny<int>())
                .Callback((int port) => Port = port);
            
            Setup(_ => _.Connect(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            Setup(_ => _.Disconnect(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            Setup(_ => _.DirectoryExists(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string directory, CancellationToken _) => _directories.ContainsKey(directory));
            
            Setup(_ => _.CreateDirectory(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback((string directory, CancellationToken _) => _directories[directory] = 1)
                .ReturnsAsync((string directory, CancellationToken _) => true);

            Setup(_ => _.UploadStream(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<FtpRemoteExists>(), It.IsAny<bool>(), It.IsAny<IProgress<FtpProgress>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Stream content, string path, FtpRemoteExists _, bool createDir, IProgress<FtpProgress> progress, CancellationToken _) => UploadStream(content, path, createDir, progress));

            Setup(_ => _.DownloadStream(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<IProgress<FtpProgress>>(), It.IsAny<CancellationToken>(), It.IsAny<long>()))
                .ReturnsAsync((Stream content, string path, long _, IProgress<FtpProgress> progress, CancellationToken _, long _) => DownloadStream(content, path, progress));

            Setup(_ => _.FileExists(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string path, CancellationToken _) => FileExists(path));

            Setup(_ => _.DeleteFile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback((string path, CancellationToken _) => DeleteFile(path))
                .Returns(Task.CompletedTask);
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
            if (_deleteFileException is not null)
                throw _deleteFileException;
            _files.TryRemove(path, out var _);
        }

        public void AddDirectory(string directory) => _directories[directory] = 1;
        public void AddFile(string path, byte[] data) => _files[path] = data;
        public void WithUploadException(Exception? exception) => _uploadException = exception;
        public void WithUploadFailure() => _uploadFailure = true;
        public void WithDownloadException(Exception? exception) => _downloadException = exception;
        public void WithDownloadFailure() => _downloadFailure = true;
        public void WithFileExistsException(Exception? exception) => _fileExistsException = exception;
        public void WithDeleteFileException(InvalidDataException exception) => _deleteFileException = exception;
    }
}
