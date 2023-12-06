namespace Microservices.Shared.CloudFiles;

/// <summary>
/// Provides the ability to read and write files saved in cloud file storage.
/// </summary>
public interface ICloudFiles
{
    /// <summary>
    /// Save a file to cloud storage.
    /// </summary>
    /// <param name="container">The file container. This would be a Bucket for AWS S3, or a directory for FTP.</param>
    /// <param name="name">The file name. This would be a Key for AWS S3, or a filename for FTP.</param>
    /// <param name="content">The source for file content to upload.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Whether the file was uploaded successfully.</returns>
    Task<bool> UploadFileAsync(string container, string name, Stream content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a file from cloud storage.
    /// </summary>
    /// <param name="container">The file container. This would be a Bucket for AWS S3, or a directory for FTP.</param>
    /// <param name="name">The file name. This would be a Key for AWS S3, or a filename for FTP.</param>
    /// <param name="content">The target for the file content to download.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Whether the file was downloaded successfully.</returns>
    Task<bool> DownloadFileAsync(string container, string name, Stream content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determine whether a file exists in cloud storage.
    /// </summary>
    /// <param name="container">The file container. This would be a Bucket for AWS S3, or a directory for FTP.</param>
    /// <param name="name">The file name. This would be a Key for AWS S3, or a filename for FTP.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Whether the file exists.</returns>
    Task<bool> FileExistsAsync(string container, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file from cloud storage.
    /// </summary>
    /// <param name="container">The file container. This would be a Bucket for AWS S3, or a directory for FTP.</param>
    /// <param name="name">The file name. This would be a Key for AWS S3, or a filename for FTP.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>Whether the file was deleted.</returns>
    Task<bool> DeleteFileAsync(string container, string name, CancellationToken cancellationToken = default);
}
