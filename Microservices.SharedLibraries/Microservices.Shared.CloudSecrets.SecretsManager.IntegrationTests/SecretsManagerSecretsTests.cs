namespace Microservices.Shared.CloudSecrets.SecretsManager.IntegrationTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "SecretsManager")]
public class SecretsManagerSecretsTests : IDisposable
{
    private readonly SecretsManagerSecretsTestsContext _context = new();
    private readonly Fixture _fixture = new();
    private bool _disposedValue;

    private const string _vault = "infrastructure";
    private const string _secret = "rabbit.user";
    private const string _value = "admin";

    [SetUp]
    public void Setup() => SecretsManagerSecrets.ClearCache();

    [Test]
    public async Task GetSecretsAsync_returns_secrets_when_exist()
    {
        // http://localhost:10083/secrets/vaults/infrastructure

        // HTTP/1.1 200 OK
        // Content-Type: application/json; charset=utf-8
        // Date: Wed, 29 Jan 2025 15:48:01 GMT
        // Server: Kestrel
        // Transfer-Encoding: chunked
        //
        // {"rabbit.user":"admin","rabbit.password":"P@ssw0rd","rabbit.vhost":"microservices"}
        var secrets = await _context.Sut.GetSecretsAsync(_vault);
        secrets.ShouldNotBeNull();
        secrets.ShouldNotBeEmpty();
    }

    [Test]
    public async Task GetSecretsAsync_returns_empty_when_does_not_exist()
    {
        // http://localhost:10083/secrets/vaults/infrastructure

        // HTTP/1.1 200 OK
        // Content-Type: application/json; charset=utf-8
        // Date: Wed, 29 Jan 2025 15:48:01 GMT
        // Server: Kestrel
        // Transfer-Encoding: chunked
        //
        // {}
        var vault = _fixture.Create<string>();
        var secrets = await _context.Sut.GetSecretsAsync(vault);
        secrets.ShouldBeEmpty();
    }

    [Test]
    public async Task GetSecretsAsync_returns_empty_when_forbidden()
    {
        _context.WithForbiddenResponse();
        var secrets = await _context.Sut.GetSecretsAsync(_vault);
        secrets.ShouldBeEmpty();
    }

    [Test]
    public async Task GetSecretsAsync_returns_empty_when_problem()
    {
        _context.WithProblemResponse();
        var secrets = await _context.Sut.GetSecretsAsync(_vault);
        secrets.ShouldBeEmpty();
    }

    [Test]
    public async Task GetSecretsAsync_returns_empty_when_unknown_base_url()
    {
        _context.WithUnknownHost();
        var secrets = await _context.Sut.GetSecretsAsync(_vault);
        secrets.ShouldBeEmpty();
    }

    [Test]
    public async Task GetSecretsAsync_logs_warning_when_unsuccessful()
    {
        _context.WithForbiddenResponse();
        await _context.Sut.GetSecretsAsync(_vault);
        _context.AssertWarningLogged($"Failed GET request for /secrets/vaults/{_vault}. Response Forbidden.");
    }

    [Test]
    public async Task GetSecretValueAsync_returns_secrets_when_exist()
    {
        var value = await _context.Sut.GetSecretValueAsync(_vault, _secret);
        value.ShouldBe(_value);
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_vault_does_not_exist()
    {
        var vault = _fixture.Create<string>();
        var value = await _context.Sut.GetSecretValueAsync(vault, _secret);
        value.ShouldBeNull();
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_secret_does_not_exist()
    {
        var secret = _fixture.Create<string>();
        var value = await _context.Sut.GetSecretValueAsync(_vault, secret);
        value.ShouldBeNull();
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_forbidden()
    {
        _context.WithForbiddenResponse();
        var value = await _context.Sut.GetSecretValueAsync(_vault, _secret);
        value.ShouldBeNull();
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_problem()
    {
        _context.WithProblemResponse();
        var value = await _context.Sut.GetSecretValueAsync(_vault, _secret);
        value.ShouldBeNull();
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_unknown_base_url()
    {
        _context.WithUnknownHost();
        var value = await _context.Sut.GetSecretValueAsync(_vault, _secret);
        value.ShouldBeNull();
    }

    [Test]
    public async Task GetSecretValueAsync_logs_warning_when_unsuccessful()
    {
        _context.WithForbiddenResponse();
        await _context.Sut.GetSecretValueAsync(_vault, _secret);
        _context.AssertWarningLogged($"Failed GET request for /secrets/vaults/{_vault}. Response Forbidden.");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _context.Dispose();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
