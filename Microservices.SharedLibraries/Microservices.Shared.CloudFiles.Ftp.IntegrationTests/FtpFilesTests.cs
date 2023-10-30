﻿using AutoFixture;
using System.Text;

namespace Microservices.Shared.CloudFiles.Ftp.IntegrationTests
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "Ftp")]
    public class FtpFilesTests : IDisposable
    {
        private readonly FtpFilesTestsContext _context = new();
        private readonly Fixture _fixture = new();
        private bool _disposedValue;

        [Test]
        public async Task EndToEndTest()
        {
            var directory = $"/tests/{_fixture.Create<string>()}";
            var file = $"{_fixture.Create<string>()}.txt";

            var fileExists = await _context.Sut.FileExistsAsync(directory, file);
            Assert.That(fileExists, Is.False, "File should not exist at the start of the test.");

            var content = _fixture.Create<string>();
            var contentBytes = Encoding.UTF8.GetBytes(content);
            using var source = new MemoryStream();
            source.Write(contentBytes);
            source.Seek(0, SeekOrigin.Begin);

            var status = await _context.Sut.UploadFileAsync(directory, file, source);
            Assert.That(status, Is.True, "Failed to upload file.");

            fileExists = await _context.Sut.FileExistsAsync(directory, file);
            Assert.That(fileExists, Is.True, "File should exist after upload.");

            using var target = new MemoryStream();
            status = await _context.Sut.DownloadFileAsync(directory, file, target);
            Assert.That(status, Is.True, "Failed to download file.");

            fileExists = await _context.Sut.FileExistsAsync(directory, file);
            Assert.That(fileExists, Is.True, "File should still exist after download.");

            status = await _context.Sut.DeleteFileAsync(directory, file);
            Assert.That(status, Is.True, "Failed to delete file.");

            fileExists = await _context.Sut.FileExistsAsync(directory, file);
            Assert.That(fileExists, Is.False, "File should not exist after delete.");

            var downloadedBytes = target.ToArray();
            Assert.That(downloadedBytes.SequenceEqual(contentBytes), Is.True, "Downloaded different bytes");

            var downloaded = Encoding.UTF8.GetString(downloadedBytes);
            Assert.That(downloaded, Is.EqualTo(content), "Downloaded content is different");
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
}
