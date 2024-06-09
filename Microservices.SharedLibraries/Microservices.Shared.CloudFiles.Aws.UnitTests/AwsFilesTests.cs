namespace Microservices.Shared.CloudFiles.Aws.UnitTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "S3")]
internal class AwsFilesTests
{
    private readonly AwsFilesTestsContext _context = new();
    private readonly Fixture _fixture = new();

    [Test]
    public void UploadFileAsync_guards_against_missing_container_argument()
    {
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        Assert.That(async () => await _context.Sut.UploadFileAsync(string.Empty, name, content), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input container was empty. (Parameter 'container')"));
    }

    [Test]
    public void UploadFileAsync_guards_against_missing_name_argument()
    {
        var container = _fixture.Create<string>();
        using var content = new MemoryStream();
        Assert.That(async () => await _context.Sut.UploadFileAsync(container, string.Empty, content), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input name was empty. (Parameter 'name')"));
    }

    [Test]
    public void UploadFileAsync_guards_against_missing_content_argument()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        Assert.That(async () => await _context.Sut.UploadFileAsync(container, name, null!), Throws.TypeOf<ArgumentNullException>().With.Property("Message").EqualTo("Value cannot be null. (Parameter 'content')"));
    }

    [Test]
    public async Task UploadFileAsync_creates_directory_if_not_exists()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        await _context.Sut.UploadFileAsync(container, name, content);
        _context.AssertBucketCreated(container);
    }

    [Test]
    public async Task UploadFileAsync_does_not_create_directory_if_exists()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        _context.WithBucket(container);
        await _context.Sut.UploadFileAsync(container, name, content);
        _context.AssertBucketNotCreated(container);
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
        Assert.That(result, Is.True);
    }

    [Test]
    public void UploadFilesAsync_rethrows_exception()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        var exception = new InvalidDataException(_fixture.Create<string>());
        _context.WithUploadException(exception);
        Assert.That(async () => await _context.Sut.UploadFileAsync(container, name, content), Throws.TypeOf<InvalidDataException>().With.Property("Message").EqualTo(exception.Message));
    }

    [Test]
    public void DownloadFileAsync_guards_against_missing_container_argument()
    {
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        Assert.That(async () => await _context.Sut.DownloadFileAsync(string.Empty, name, content), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input container was empty. (Parameter 'container')"));
    }

    [Test]
    public void DownloadFileAsync_guards_against_missing_name_argument()
    {
        var container = _fixture.Create<string>();
        using var content = new MemoryStream();
        Assert.That(async () => await _context.Sut.DownloadFileAsync(container, string.Empty, content), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input name was empty. (Parameter 'name')"));
    }

    [Test]
    public void DownloadFileAsync_guards_against_missing_content_argument()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        Assert.That(async () => await _context.Sut.DownloadFileAsync(container, name, null!), Throws.TypeOf<ArgumentNullException>().With.Property("Message").EqualTo("Value cannot be null. (Parameter 'content')"));
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
        Assert.That(data.SequenceEqual(content.ToArray()), Is.True);
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
        Assert.That(result, Is.True);
    }

    [Test]
    public void DownloadFileAsync_rethrows_exception()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        using var content = new MemoryStream();
        var exception = new InvalidDataException(_fixture.Create<string>());
        _context.WithDownloadException(exception);
        Assert.That(async () => await _context.Sut.DownloadFileAsync(container, name, content), Throws.TypeOf<InvalidDataException>().With.Property("Message").EqualTo(exception.Message));
    }

    [Test]
    public void FileExistsAsync_guards_against_missing_container_argument()
    {
        var name = _fixture.Create<string>();
        Assert.That(async () => await _context.Sut.FileExistsAsync(string.Empty, name), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input container was empty. (Parameter 'container')"));
    }

    [Test]
    public void FileExistsAsync_guards_against_missing_name_argument()
    {
        var container = _fixture.Create<string>();
        Assert.That(async () => await _context.Sut.FileExistsAsync(container, string.Empty), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input name was empty. (Parameter 'name')"));
    }

    [Test]
    public async Task FileExistsAsync_returns_true_if_file_exists()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var data = _fixture.Create<byte[]>();
        _context.WithFile(container, name, data);
        var result = await _context.Sut.FileExistsAsync(container, name);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task FileExistsAsync_returns_true_if_file_does_not_exist()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        _context.WithBucket(container);
        var result = await _context.Sut.FileExistsAsync(container, name);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task FileExistsAsync_returns_true_if_directory_does_not_exist()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var result = await _context.Sut.FileExistsAsync(container, name);
        Assert.That(result, Is.False);
    }

    [Test]
    public void FileExistsAsync_rethrows_exception()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var data = _fixture.Create<byte[]>();
        _context.WithFile(container, name, data);
        var exception = new InvalidDataException(_fixture.Create<string>());
        _context.WithFileExistsException(exception);
        Assert.That(async () => await _context.Sut.FileExistsAsync(container, name), Throws.TypeOf<InvalidDataException>().With.Property("Message").EqualTo(exception.Message));
    }

    [Test]
    public void DeleteFileAsync_guards_against_missing_container_argument()
    {
        var name = _fixture.Create<string>();
        Assert.That(async () => await _context.Sut.DeleteFileAsync(string.Empty, name), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input container was empty. (Parameter 'container')"));
    }

    [Test]
    public void DeleteFileAsync_guards_against_missing_name_argument()
    {
        var container = _fixture.Create<string>();
        Assert.That(async () => await _context.Sut.DeleteFileAsync(container, string.Empty), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input name was empty. (Parameter 'name')"));
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
        Assert.That(result, Is.True);
    }

    [Test]
    public void DeleteFileAsync_rethrows_exception()
    {
        var container = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var data = _fixture.Create<byte[]>();
        _context.WithFile(container, name, data);
        var exception = new InvalidDataException(_fixture.Create<string>());
        _context.WithDeleteFileException(exception);
        Assert.That(async () => await _context.Sut.DeleteFileAsync(container, name), Throws.TypeOf<InvalidDataException>().With.Property("Message").EqualTo(exception.Message));
    }

    [Test]
    public void GetBucketAndKey_guards_against_missing_container_argument()
    {
        var name = _fixture.Create<string>();
        Assert.That(() => _context.Sut.GetBucketAndKey(string.Empty, name), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input container was empty. (Parameter 'container')"));
    }

    [Test]
    public void GetBucketAndKey_guards_against_missing_name_argument()
    {
        var container = _fixture.Create<string>();
        Assert.That(() => _context.Sut.GetBucketAndKey(container, string.Empty), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input name was empty. (Parameter 'name')"));
    }

    [Test]
    public void GetBucketAndKey_removes_backslashes_from_container()
    {
        var a = _fixture.Create<string>();
        var b = _fixture.Create<string>();
        var c = _fixture.Create<string>();
        var container = $"/\\/{a.ToUpper()}\\{b}/\\/{c}/\\/";
        var name = _fixture.Create<string>();

        var (bucket, _) = _context.Sut.GetBucketAndKey(container, name);

        var expected = $"{a}.{b}...{c}".ToLower();
        Assert.That(bucket, Is.EqualTo(expected));
    }

    [Test]
    public void GetBucketAndKey_removes_backslashes_from_name()
    {
        var a = _fixture.Create<string>();
        var b = _fixture.Create<string>();
        var c = _fixture.Create<string>();
        var container = _fixture.Create<string>(); 
        var name = $"/\\/{a}\\{b}/\\/{c}/\\/";

        var (_, key) = _context.Sut.GetBucketAndKey(container, name);

        var expected = $"{a}/{b}/{c}";
        Assert.That(key, Is.EqualTo(expected));
    }
}
