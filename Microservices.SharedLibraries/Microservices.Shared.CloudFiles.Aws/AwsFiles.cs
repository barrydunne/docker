using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Microservices.Shared.CloudFiles.Aws;

/// <inheritdoc/>
public class AwsFiles : ICloudFiles
{
    private readonly S3Region _s3Region;
    private readonly IAmazonS3 _amazonS3;
    private readonly ILogger _logger;

    private static readonly ActivitySource _activitySource = new(CloudFiles.ActivitySourceName);

    /// <summary>
    /// Initializes a new instance of the <see cref="AwsFiles"/> class.
    /// </summary>
    /// <param name="options">The configuration options.</param>
    /// <param name="amazonS3">The Amazon S3 client to use.</param>
    /// <param name="logger">The logger to write to.</param>
    public AwsFiles(IOptions<AwsFilesOptions> options, IAmazonS3 amazonS3, ILogger<AwsFiles> logger)
    {
        _s3Region = new S3Region(options.Value.Region);
        _amazonS3 = amazonS3;
        _logger = logger;

        // Use path style addressing for localstack.
        //   http://s3.eu-west-1.amazonaws.com/bucketname
        // instead of
        //   http://bucketname.s3.eu-west-1.amazonaws.com
        if (_amazonS3.Config is AmazonS3Config config)
            config.ForcePathStyle = true;
    }

    /// <inheritdoc/>
    public async Task<bool> UploadFileAsync(string container, string name, Stream content, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity("S3 Upload", ActivityKind.Client);
        var (bucket, key) = GetBucketAndKey(container, name);
        Guard.Against.Null(content, nameof(content));
        try
        {
            _logger.LogInformation("Checking bucket {Bucket} exists", bucket);
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucket);
            if (!bucketExists)
            {
                var request = new PutBucketRequest { BucketName = bucket, BucketRegion = _s3Region };
                _logger.LogInformation("Creating bucket {Bucket} in {Region}", bucket, request.BucketRegion);
                await _amazonS3.PutBucketAsync(request, cancellationToken);
            }

            _logger.LogInformation("Uploading file {Key}", key);
            await _amazonS3.UploadObjectFromStreamAsync(bucket, key, content, null, cancellationToken);
            return true;
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
        using var activity = _activitySource.StartActivity("S3 Download", ActivityKind.Client);
        var (bucket, key) = GetBucketAndKey(container, name);
        Guard.Against.Null(content, nameof(content));
        try
        {
            _logger.LogInformation("Downloading file {Bucket}/{Key}", bucket, key);
            using var stream = await _amazonS3.GetObjectStreamAsync(bucket, key, null, cancellationToken);
            await stream.CopyToAsync(content, cancellationToken);
            _logger.LogInformation("Download complete");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download {Container}/{File}.", container, name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> FileExistsAsync(string container, string name, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity("S3 File exists", ActivityKind.Client);
        var (bucket, key) = GetBucketAndKey(container, name);
        try
        {
            _logger.LogInformation("Checking bucket {Bucket}", bucket);
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucket);
            _logger.LogInformation("Bucket {Bucket} exists: {Exists}", bucket, bucketExists);

            if (bucketExists)
            {
                _logger.LogInformation("Checking file {Key}", key);
                GetObjectMetadataResponse? metadata = null;
                try
                {
                    metadata = await _amazonS3.GetObjectMetadataAsync(bucket, key, cancellationToken);
                }
                catch
                {
                    // Exception happens if the file does not exist
                }
                _logger.LogInformation("File {Key} exists: {Exists}", key, metadata is not null);
                return metadata is not null;
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
        using var activity = _activitySource.StartActivity("S3 Delete");
        var (bucket, key) = GetBucketAndKey(container, name);
        try
        {
            _logger.LogInformation("Deleting file {Bucket}/{Key}", bucket, key);
            await _amazonS3.DeleteObjectAsync(bucket, key, cancellationToken);
            _logger.LogInformation("File {Bucket}/{Key} deleted.", bucket, key);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete {Container}/{File}.", container, name);
            throw;
        }
    }

    /// <summary>
    /// Gets the bucket and key for the specified container and name.
    /// </summary>
    /// <param name="container">The container name.</param>
    /// <param name="name">The file name.</param>
    /// <returns>A tuple containing the bucket and key.</returns>
    internal (string Bucket, string Key) GetBucketAndKey(string container, string name)
    {
        Guard.Against.NullOrEmpty(container, nameof(container));
        Guard.Against.NullOrEmpty(name, nameof(name));
        var bucket = container.Replace('\\', '/').Trim('/').Replace('/', '.').ToLower();
        var key = name.Replace('\\', '/').Trim('/');
        while (key.Contains("//"))
            key = key.Replace("//", "/");
        return (bucket, key);
    }
}
