﻿using System.Text;

namespace Microservices.Shared.CloudFiles.Aws.IntegrationTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "S3")]
public class AwsFilesTests : IDisposable
{
    private readonly AwsFilesTestsContext _context = new();
    private readonly Fixture _fixture = new();
    private bool _disposedValue;

    [Test]
    public async Task EndToEndTest()
    {
        var bucket = _fixture.Create<string>();
        var key = $"{_fixture.Create<string>()}/{_fixture.Create<string>()}.txt";

        var fileExists = await _context.Sut.FileExistsAsync(bucket, key);
        fileExists.ShouldBeFalse("File should not exist at the start of the test.");

        var content = _fixture.Create<string>();
        var contentBytes = Encoding.UTF8.GetBytes(content);
        using var source = new MemoryStream();
        source.Write(contentBytes);
        source.Seek(0, SeekOrigin.Begin);

        var status = await _context.Sut.UploadFileAsync(bucket, key, source);
        status.ShouldBeTrue("Failed to upload file.");

        fileExists = await _context.Sut.FileExistsAsync(bucket, key);
        fileExists.ShouldBeTrue("File should exist after upload.");

        using var target = new MemoryStream();
        status = await _context.Sut.DownloadFileAsync(bucket, key, target);
        status.ShouldBeTrue("Failed to download file.");

        fileExists = await _context.Sut.FileExistsAsync(bucket, key);
        fileExists.ShouldBeTrue("File should still exist after download.");

        status = await _context.Sut.DeleteFileAsync(bucket, key);
        status.ShouldBeTrue("Failed to delete file.");

        fileExists = await _context.Sut.FileExistsAsync(bucket, key);
        fileExists.ShouldBeFalse("File should not exist after delete.");

        var downloadedBytes = target.ToArray();
        downloadedBytes.ShouldBe(contentBytes, "Downloaded different bytes");

        var downloaded = Encoding.UTF8.GetString(downloadedBytes);
        downloaded.ShouldBe(content, "Downloaded content is different");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _context?.Dispose();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
