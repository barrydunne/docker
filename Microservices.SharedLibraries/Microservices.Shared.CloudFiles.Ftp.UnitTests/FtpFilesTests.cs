namespace Microservices.Shared.CloudFiles.Ftp.UnitTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Ftp")]
internal class FtpFilesTests
{
    private readonly FtpFilesTestsContext _context = new();
    private readonly Fixture _fixture = new();

    [Test]
    public void Constructor_sets_host_from_options()
    {
        var _ = _context.Sut;
        _context.AssertHostSet();
    }

    [Test]
    public void Constructor_sets_port_from_options()
    {
        var _ = _context.Sut;
        _context.AssertPortSet();
    }

    [Test]
    public async Task UploadFileAsync_guards_against_missing_container_argument()
    {
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        async Task<bool> func() => await _context.Sut.UploadFileAsync(string.Empty, name, content);
        var ex = await func().ShouldThrowAsync<ArgumentException>();
        ex.Message.ShouldBe("Required input container was empty. (Parameter 'container')");
    }

    [Test]
    public async Task UploadFileAsync_guards_against_missing_name_argument()
    {
        var container = _fixture.Create<string>();
        using var content = new MemoryStream();
        async Task<bool> func() => await _context.Sut.UploadFileAsync(container, string.Empty, content);
        var ex = await func().ShouldThrowAsync<ArgumentException>();
        ex.Message.ShouldBe("Required input name was empty. (Parameter 'name')");
    }

    [Test]
    public async Task UploadFileAsync_guards_against_missing_content_argument()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        async Task<bool> func() => await _context.Sut.UploadFileAsync(container, name, null!);
        var ex = await func().ShouldThrowAsync<ArgumentNullException>();
        ex.Message.ShouldBe("Value cannot be null. (Parameter 'content')");
    }

    [Test]
    public async Task UploadFileAsync_creates_directory_if_not_exists()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        await _context.Sut.UploadFileAsync(container, name, content);
        _context.AssertDirectoryCreated(container);
    }

    [Test]
    public async Task UploadFileAsync_does_not_create_directory_if_exists()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        _context.WithDirectory(container);
        await _context.Sut.UploadFileAsync(container, name, content);
        _context.AssertDirectoryNotCreated(container);
    }

    [Test]
    public async Task UploadFileAsync_uploads_file()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        var data = _fixture.Create<byte[]>();
        content.Write(data);
        content.Seek(0, SeekOrigin.Begin);
        await _context.Sut.UploadFileAsync(container, name, content);
        _context.AssertFileUploaded(container, name, data);
    }

    [Test]
    public async Task UploadFileAsync_returns_true_on_success()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        var data = _fixture.Create<byte[]>();
        content.Write(data);
        content.Seek(0, SeekOrigin.Begin);
        var result = await _context.Sut.UploadFileAsync(container, name, content);
        result.ShouldBeTrue();
    }

    [Test]
    public async Task UploadFileAsync_returns_false_on_failure()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        var data = _fixture.Create<byte[]>();
        content.Write(data);
        content.Seek(0, SeekOrigin.Begin);
        _context.WithUploadFailure();
        var result = await _context.Sut.UploadFileAsync(container, name, content);
        result.ShouldBeFalse();
    }

    [Test]
    public async Task UploadFilesAsync_rethrows_exception()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        var exception = new InvalidDataException(_fixture.Create<string>());
        _context.WithUploadException(exception);
        async Task<bool> func() => await _context.Sut.UploadFileAsync(container, name, content);
        var ex = await func().ShouldThrowAsync<InvalidDataException>();
        ex.Message.ShouldBe(exception.Message);
    }

    [Test]
    public async Task DownloadFileAsync_guards_against_missing_container_argument()
    {
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        async Task<bool> func() => await _context.Sut.DownloadFileAsync(string.Empty, name, content);
        var ex = await func().ShouldThrowAsync<ArgumentException>();
        ex.Message.ShouldBe("Required input container was empty. (Parameter 'container')");
    }

    [Test]
    public async Task DownloadFileAsync_guards_against_missing_name_argument()
    {
        var container = _fixture.Create<string>();
        using var content = new MemoryStream();
        async Task<bool> func() => await _context.Sut.DownloadFileAsync(container, string.Empty, content);
        var ex = await func().ShouldThrowAsync<ArgumentException>();
        ex.Message.ShouldBe("Required input name was empty. (Parameter 'name')");
    }

    [Test]
    public async Task DownloadFileAsync_guards_against_missing_content_argument()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        async Task<bool> func() => await _context.Sut.DownloadFileAsync(container, name, null!);
        var ex = await func().ShouldThrowAsync<ArgumentNullException>();
        ex.Message.ShouldBe("Value cannot be null. (Parameter 'content')");
    }

    [Test]
    public async Task DownloadFileAsync_downloads_file()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        var data = _fixture.Create<byte[]>();
        _context.WithFile(container, name, data);
        await _context.Sut.DownloadFileAsync(container, name, content);
        data.ShouldBe(content.ToArray());
    }

    [Test]
    public async Task DownloadFileAsync_returns_true_on_success()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        var data = _fixture.Create<byte[]>();
        _context.WithFile(container, name, data);
        var result = await _context.Sut.DownloadFileAsync(container, name, content);
        result.ShouldBeTrue();
    }

    [Test]
    public async Task DownloadFileAsync_returns_false_on_failure()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        var data = _fixture.Create<byte[]>();
        _context.WithFile(container, name, data);
        _context.WithDownloadFailure();
        var result = await _context.Sut.DownloadFileAsync(container, name, content);
        result.ShouldBeFalse();
    }

    [Test]
    public async Task DownloadFileAsync_rethrows_exception()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        var exception = new InvalidDataException(_fixture.Create<string>());
        _context.WithDownloadException(exception);
        async Task<bool> func() => await _context.Sut.DownloadFileAsync(container, name, content);
        var ex = await func().ShouldThrowAsync<InvalidDataException>();
        ex.Message.ShouldBe(exception.Message);
    }

    [Test]
    public async Task FileExistsAsync_guards_against_missing_container_argument()
    {
        var name = _fixture.Create<string>();
        async Task<bool> func() => await _context.Sut.FileExistsAsync(string.Empty, name);
        var ex = await func().ShouldThrowAsync<ArgumentException>();
        ex.Message.ShouldBe("Required input container was empty. (Parameter 'container')");
    }

    [Test]
    public async Task FileExistsAsync_guards_against_missing_name_argument()
    {
        var container = _fixture.Create<string>();
        async Task<bool> func() => await _context.Sut.FileExistsAsync(container, string.Empty);
        var ex = await func().ShouldThrowAsync<ArgumentException>();
        ex.Message.ShouldBe("Required input name was empty. (Parameter 'name')");
    }

    [Test]
    public async Task FileExistsAsync_returns_true_if_file_exists()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var data = _fixture.Create<byte[]>();
        _context.WithFile(container, name, data);
        var result = await _context.Sut.FileExistsAsync(container, name);
        result.ShouldBeTrue();
    }

    [Test]
    public async Task FileExistsAsync_returns_true_if_file_does_not_exist()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        _context.WithDirectory(container);
        var result = await _context.Sut.FileExistsAsync(container, name);
        result.ShouldBeFalse();
    }

    [Test]
    public async Task FileExistsAsync_returns_true_if_directory_does_not_exist()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var result = await _context.Sut.FileExistsAsync(container, name);
        result.ShouldBeFalse();
    }

    [Test]
    public async Task FileExistsAsync_rethrows_exception()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var data = _fixture.Create<byte[]>();
        _context.WithFile(container, name, data);
        var exception = new InvalidDataException(_fixture.Create<string>());
        _context.WithFileExistsException(exception);
        async Task<bool> func() => await _context.Sut.FileExistsAsync(container, name);
        var ex = await func().ShouldThrowAsync<InvalidDataException>();
        ex.Message.ShouldBe(exception.Message);
    }

    [Test]
    public async Task DeleteFileAsync_guards_against_missing_container_argument()
    {
        var name = _fixture.Create<string>();
        async Task<bool> func() => await _context.Sut.DeleteFileAsync(string.Empty, name);
        var ex = await func().ShouldThrowAsync<ArgumentException>();
        ex.Message.ShouldBe("Required input container was empty. (Parameter 'container')");
    }

    [Test]
    public async Task DeleteFileAsync_guards_against_missing_name_argument()
    {
        var container = _fixture.Create<string>();
        async Task<bool> func() => await _context.Sut.DeleteFileAsync(container, string.Empty);
        var ex = await func().ShouldThrowAsync<ArgumentException>();
        ex.Message.ShouldBe("Required input name was empty. (Parameter 'name')");
    }

    [Test]
    public async Task DeleteFileAsync_deletes_file()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var data = _fixture.Create<byte[]>();
        _context.WithFile(container, name, data);
        await _context.Sut.DeleteFileAsync(container, name);
        _context.AssertFileDeleted(container, name);
    }

    [Test]
    public async Task DeleteFileAsync_returns_true_if_file_deleted()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var data = _fixture.Create<byte[]>();
        _context.WithFile(container, name, data);
        var result = await _context.Sut.DeleteFileAsync(container, name);
        result.ShouldBeTrue();
    }

    [Test]
    public async Task DeleteFileAsync_rethrows_exception()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var data = _fixture.Create<byte[]>();
        _context.WithFile(container, name, data);
        var exception = new InvalidDataException(_fixture.Create<string>());
        _context.WithDeleteFileException(exception);
        async Task<bool> func() => await _context.Sut.DeleteFileAsync(container, name);
        var ex = await func().ShouldThrowAsync<InvalidDataException>();
        ex.Message.ShouldBe(exception.Message);
    }
}
