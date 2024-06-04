using Ardalis.GuardClauses;
using FluentFTP;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Microservices.Shared.CloudFiles.Ftp;

/// <inheritdoc/>
public class FtpFiles : ICloudFiles
{
    private readonly FtpFilesOptions _options;
    private readonly IAsyncFtpClient _asyncFtpClient;
    private readonly ILogger _logger;

    private static readonly ActivitySource _activitySource = new("Microservices.Shared.CloudFiles.Ftp");

    /// <summary>
    /// Initializes a new instance of the <see cref="FtpFiles"/> class.
    /// </summary>
    /// <param name="options">The connection configuration options.</param>
    /// <param name="asyncFtpClient">The FTP client to use.</param>
    /// <param name="logger">The logger to write to.</param>
    public FtpFiles(IOptions<FtpFilesOptions> options, IAsyncFtpClient asyncFtpClient, ILogger<FtpFiles> logger)
    {
        _options = options.Value;
        _asyncFtpClient = asyncFtpClient;
        _asyncFtpClient.Host = _options.Host;
        _asyncFtpClient.Port = _options.Port;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<bool> UploadFileAsync(string container, string name, Stream content, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity("FTP Upload", ActivityKind.Client);
        var (remoteDirectory, remotePath) = GetRemoteDirectoryAndPath(container, name);
        Guard.Against.Null(content, nameof(content));
        try
        {
            _logger.LogInformation("Checking directory {Directory}", remoteDirectory);
            await _asyncFtpClient.Connect(cancellationToken);
            var directoryExists = await _asyncFtpClient.DirectoryExists(remoteDirectory, cancellationToken);
            if (!directoryExists)
            {
                _logger.LogInformation("Creating directory {Directory}", remoteDirectory);
                await _asyncFtpClient.CreateDirectory(remoteDirectory, cancellationToken);
            }

            _logger.LogInformation("Uploading file {Path}", remotePath);
            var status = await _asyncFtpClient.UploadStream(content, remotePath, FtpRemoteExists.Overwrite, true, new ProgressReporter(_logger), cancellationToken);
            _logger.LogInformation("Upload status: {Status}", status);
            await _asyncFtpClient.Disconnect(cancellationToken);
            return status == FtpStatus.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload {Container}/{File}.", container, name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DownloadFileAsync(string container, string name, Stream content, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity("FTP Download", ActivityKind.Client);
        var (_, remotePath) = GetRemoteDirectoryAndPath(container, name);
        Guard.Against.Null(content, nameof(content));
        try
        {
            _logger.LogInformation("Downloading file {Path}", remotePath);
            await _asyncFtpClient.Connect(cancellationToken);
            var status = await _asyncFtpClient.DownloadStream(content, remotePath, 0, new ProgressReporter(_logger), cancellationToken);
            _logger.LogInformation("Download status: {Status}", status ? FtpStatus.Success : FtpStatus.Failed);
            await _asyncFtpClient.Disconnect(cancellationToken);
            return status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload {Container}/{File}.", container, name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> FileExistsAsync(string container, string name, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity("FTP File exists", ActivityKind.Client);
        var (remoteDirectory, remotePath) = GetRemoteDirectoryAndPath(container, name);
        try
        {
            _logger.LogInformation("Checking directory {Directory}", remoteDirectory);
            await _asyncFtpClient.Connect(cancellationToken);
            var directoryExists = await _asyncFtpClient.DirectoryExists(remoteDirectory, cancellationToken);
            _logger.LogInformation("Directory {Directory} exists: {Exists}", remoteDirectory, directoryExists);

            if (directoryExists)
            {
                _logger.LogInformation("Checking file {Path}", remotePath);
                var fileExists = await _asyncFtpClient.FileExists(remotePath, cancellationToken);
                _logger.LogInformation("File {Path} exists: {Exists}", remotePath, fileExists);
                await _asyncFtpClient.Disconnect(cancellationToken);

                return fileExists;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check {Container}/{File}.", container, name);
            throw;
        }
        return false;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteFileAsync(string container, string name, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity("FTP Delete");
        var (_, remotePath) = GetRemoteDirectoryAndPath(container, name);
        try
        {
            _logger.LogInformation("Deleting file {Path}", remotePath);
            await _asyncFtpClient.Connect(cancellationToken);
            await _asyncFtpClient.DeleteFile(remotePath, cancellationToken);
            _logger.LogInformation("File {Path} deleted.", remotePath);
            await _asyncFtpClient.Disconnect(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete {Container}/{File}.", container, name);
            throw;
        }
    }

    private (string RemoteDirectory, string RemotePath) GetRemoteDirectoryAndPath(string container, string name)
    {
        Guard.Against.NullOrEmpty(container, nameof(container));
        Guard.Against.NullOrEmpty(name, nameof(name));
        var remoteDir = $"{_options.BaseDir}/{container.Trim('/')}";
        var remotePath = $"{remoteDir}/{name.Trim('/')}";
        return (remoteDir, remotePath);
    }
}
