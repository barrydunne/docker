using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Collections.Concurrent;
using System.Net;

namespace Microservices.Shared.CloudFiles.Aws.UnitTests;

internal class AwsFilesTestsContext
{
    private readonly Fixture _fixture;
    private readonly AwsFilesOptions _options;
    private readonly IOptions<AwsFilesOptions> _mockOptions;
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte[]>> _files;
    private readonly IAmazonS3 _mockAmazonS3;
    private readonly MockLogger<AwsFiles> _mockLogger;

    internal AwsFiles Sut => new(_mockOptions, _mockAmazonS3, _mockLogger);

    internal AwsFilesTestsContext()
    {
        _fixture = new();

        _options = _fixture.Create<AwsFilesOptions>();
        _mockOptions = Substitute.For<IOptions<AwsFilesOptions>>();
        _mockOptions.Value.Returns(_options);

        _files = new();

        _mockAmazonS3 = Substitute.For<IAmazonS3>();
        _mockAmazonS3
            .Config
            .Returns(_ => new AmazonS3Config());
        _mockAmazonS3
            .When(_ => _.PutBucketAsync(Arg.Any<PutBucketRequest>(), Arg.Any<CancellationToken>()))
            .Do(callInfo =>
            {
                var request = callInfo.ArgAt<PutBucketRequest>(0);
                var bucket = request.BucketName;
                _files.TryAdd(bucket, []);
            });
        _mockAmazonS3
            .When(_ => _.UploadObjectFromStreamAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<IDictionary<string, object>>(), Arg.Any<CancellationToken>()))
            .Do(callInfo =>
            {
                var bucket = callInfo.ArgAt<string>(0);
                if (!_files.ContainsKey(bucket))
                    _files.TryAdd(bucket, []);
                var key = callInfo.ArgAt<string>(1);
                var stream = callInfo.ArgAt<Stream>(2);
                using var mem = new MemoryStream();
                stream.CopyTo(mem);
                _files[bucket].TryAdd(key, mem.ToArray());
            });
        _mockAmazonS3
            .When(_ => _.DeleteObjectAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()))
            .Do(callInfo =>
            {
                var bucket = callInfo.ArgAt<string>(0);
                if (!_files.ContainsKey(bucket))
                    return;
                var key = callInfo.ArgAt<string>(1);
                _files[bucket].TryRemove(key, out _);
            });
        _mockAmazonS3
            .GetObjectStreamAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var bucket = callInfo.ArgAt<string>(0);
                var key = callInfo.ArgAt<string>(1);
                if (!_files.TryGetValue(bucket, out var files) || !files.TryGetValue(key, out var data))
                    throw new InvalidDataException();
                return new MemoryStream(data);
            });
        _mockAmazonS3
            .When(_ => _.GetACLAsync(Arg.Any<string>()))
            .Do(callInfo =>
            {
                var bucket = callInfo.ArgAt<string>(0);
                if (_files.ContainsKey(bucket))
                    return;
                throw new AmazonS3Exception("", ErrorType.Unknown, "NoSuchBucket", "", HttpStatusCode.NotFound);
            });
        _mockAmazonS3
            .GetObjectMetadataAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var bucket = callInfo.ArgAt<string>(0);
                var key = callInfo.ArgAt<string>(1);
                if (!_files.TryGetValue(bucket, out var files) || !files.TryGetValue(key, out var data))
                    throw new Exception();
                return new GetObjectMetadataResponse();
            });

        _mockLogger = new();
    }

    internal AwsFilesTestsContext WithBucket(string bucket)
    {
        if (!_files.ContainsKey(bucket))
            _files.TryAdd(bucket, []);
        return this;
    }

    internal AwsFilesTestsContext WithFile(string container, string name, byte[] data)
    {
        WithBucket(container);
        _files[container].TryAdd(name, data);
        return this;
    }

    internal AwsFilesTestsContext WithUploadException(InvalidDataException exception)
    {
        _mockAmazonS3
            .UploadObjectFromStreamAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<IDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Throws(exception);
        return this;
    }

    internal AwsFilesTestsContext WithDownloadException(InvalidDataException exception)
    {
        _mockAmazonS3
            .GetObjectStreamAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Throws(exception);
        return this;
    }

    internal AwsFilesTestsContext WithFileExistsException(InvalidDataException exception)
    {
        _mockAmazonS3
            .GetACLAsync(Arg.Any<string>())
            .Throws(exception);
        return this;
    }

    internal AwsFilesTestsContext WithDeleteFileException(InvalidDataException exception)
    {
        _mockAmazonS3
            .DeleteObjectAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Throws(exception);
        return this;
    }

    internal AwsFilesTestsContext AssertBucketCreated(string bucket) => AssertBucketCreated(bucket, 1);

    internal AwsFilesTestsContext AssertBucketNotCreated(string bucket) => AssertBucketCreated(bucket, 0);

    internal AwsFilesTestsContext AssertBucketCreated(string bucket, int times)
    {
        _mockAmazonS3
        .Received(times)
            .PutBucketAsync(Arg.Any<PutBucketRequest>(), Arg.Any<CancellationToken>());
        Assert.That(_files.TryGetValue(bucket, out var _), Is.True, "Bucket should exist");
        return this;
    }

    internal AwsFilesTestsContext AssertFileUploaded(string container, string name, byte[] data)
    {
        _mockAmazonS3
            .Received(1)
            .UploadObjectFromStreamAsync(container, name, Arg.Any<Stream>(), Arg.Any<IDictionary<string, object>>(), Arg.Any<CancellationToken>());

        Assert.That(_files.TryGetValue(container, out var bucket), Is.True, "Bucket should exist");
        byte[]? bytes = null;
        Assert.That(bucket?.TryGetValue(name, out bytes), Is.True, "Key should exist");
        Assert.That(bytes?.SequenceEqual(data), Is.True, "Data should match");
        return this;
    }

    internal AwsFilesTestsContext AssertFileDeleted(string container, string name)
    {
        _mockAmazonS3
            .Received(1)
            .DeleteObjectAsync(container, name, Arg.Any<CancellationToken>());
        return this;
    }
}
