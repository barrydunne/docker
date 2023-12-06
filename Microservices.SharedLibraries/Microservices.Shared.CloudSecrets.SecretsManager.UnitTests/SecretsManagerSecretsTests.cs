namespace Microservices.Shared.CloudSecrets.SecretsManager.UnitTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "SecretsManager")]
public class SecretsManagerSecretsTests
{
    private readonly SecretsManagerSecretsTestsContext _context = new();
    private readonly Fixture _fixture = new();

    [SetUp]
    public void Setup() => SecretsManagerSecrets.ClearCache();

    [Test]
    public async Task GetSecretsAsync_returns_secrets_when_exist()
    {
        var vault = _fixture.Create<string>();
        var vaultSecrets = _fixture.Create<Dictionary<string, string>>();
        _context.WithVault(vault, vaultSecrets);
        var secrets = await _context.Sut.GetSecretsAsync(vault);
        Assert.That(secrets, Is.EquivalentTo(vaultSecrets));
    }

    [Test]
    public async Task GetSecretsAsync_returns_secrets_when_vault_in_cache()
    {
        var vault = _fixture.Create<string>();
        var vaultSecrets = _fixture.Create<Dictionary<string, string>>();
        _context.WithVault(vault, vaultSecrets);
        await _context.Sut.GetSecretsAsync(vault);
        var secrets = await _context.Sut.GetSecretsAsync(vault);
        Assert.That(secrets, Is.EquivalentTo(vaultSecrets));
    }

    [Test]
    public async Task GetSecretsAsync_does_not_use_api_when_vault_in_cache()
    {
        var vault = _fixture.Create<string>();
        var vaultSecrets = _fixture.Create<Dictionary<string, string>>();
        _context.WithVault(vault, vaultSecrets);
        await _context.Sut.GetSecretsAsync(vault);
        await _context.Sut.GetSecretsAsync(vault);
        _context.AssertThatSecretsApiCalledOnce();
    }

    [Test]
    public async Task GetSecretsAsync_returns_empty_when_does_not_exist()
    {
        var vault = _fixture.Create<string>();
        var secrets = await _context.Sut.GetSecretsAsync(vault);
        Assert.That(secrets, Is.Empty);
    }

    [Test]
    public async Task GetSecretsAsync_returns_empty_when_forbidden()
    {
        var vault = _fixture.Create<string>();
        var vaultSecrets = _fixture.Create<Dictionary<string, string>>();
        _context
            .WithVault(vault, vaultSecrets) // Added for test clarity
            .WithForbiddenResponse();
        var secrets = await _context.Sut.GetSecretsAsync(vault);
        Assert.That(secrets, Is.Empty);
    }

    [Test]
    public async Task GetSecretsAsync_returns_empty_when_problem()
    {
        var vault = _fixture.Create<string>();
        var vaultSecrets = _fixture.Create<Dictionary<string, string>>();
        _context
            .WithVault(vault, vaultSecrets) // Added for test clarity
            .WithProblemResponse();
        var secrets = await _context.Sut.GetSecretsAsync(vault);
        Assert.That(secrets, Is.Empty);
    }

    [Test]
    public async Task GetSecretsAsync_returns_empty_when_unknown_base_url()
    {
        var vault = _fixture.Create<string>();
        _context.WithUnknownHost();
        var secrets = await _context.Sut.GetSecretsAsync(vault);
        Assert.That(secrets, Is.Empty);
    }

    [Test]
    public async Task GetSecretsAsync_returns_empty_when_exception()
    {
        var vault = _fixture.Create<string>();
        _context.WithException(_fixture.Create<string>());
        var secrets = await _context.Sut.GetSecretsAsync(vault);
        Assert.That(secrets, Is.Empty);
    }

    [Test]
    public async Task GetSecretsAsync_logs_warning_when_unsuccessful()
    {
        var vault = _fixture.Create<string>();
        _context.WithForbiddenResponse();
        await _context.Sut.GetSecretsAsync(vault);
        _context.AssertWarningLogged($"Failed GET request for /secrets/vaults/{vault}. Response Forbidden.");
    }

    [Test]
    public void GetSecretsAsync_guards_against_missing_vault_argument()
        => Assert.That(async () => await _context.Sut.GetSecretsAsync(string.Empty), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input vault was empty. (Parameter 'vault')"));

    [Test]
    public async Task GetSecretValueAsync_returns_secrets_when_exist()
    {
        var vault = _fixture.Create<string>();
        var vaultSecrets = _fixture.Create<Dictionary<string, string>>();
        _context.WithVault(vault, vaultSecrets);
        // Pick a random secret
        var secret = vaultSecrets.Keys.OrderBy(_ => Random.Shared.NextDouble()).First();
        var value = await _context.Sut.GetSecretValueAsync(vault, secret);
        Assert.That(value, Is.EqualTo(vaultSecrets[secret]));
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_vault_does_not_exist()
    {
        var vault = _fixture.Create<string>();
        var secret = _fixture.Create<string>();
        var value = await _context.Sut.GetSecretValueAsync(vault, secret);
        Assert.That(value, Is.Null);
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_secret_does_not_exist()
    {
        var vault = _fixture.Create<string>();
        var vaultSecrets = _fixture.Create<Dictionary<string, string>>();
        _context.WithVault(vault, vaultSecrets);
        var secret = _fixture.Create<string>();
        var value = await _context.Sut.GetSecretValueAsync(vault, secret);
        Assert.That(value, Is.Null);
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_forbidden()
    {
        var vault = _fixture.Create<string>();
        var secret = _fixture.Create<string>();
        _context.WithForbiddenResponse();
        var value = await _context.Sut.GetSecretValueAsync(vault, secret);
        Assert.That(value, Is.Null);
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_problem()
    {
        var vault = _fixture.Create<string>();
        var secret = _fixture.Create<string>();
        _context.WithProblemResponse();
        var value = await _context.Sut.GetSecretValueAsync(vault, secret);
        Assert.That(value, Is.Null);
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_unknown_base_url()
    {
        var vault = _fixture.Create<string>();
        var secret = _fixture.Create<string>();
        _context.WithUnknownHost();
        var value = await _context.Sut.GetSecretValueAsync(vault, secret);
        Assert.That(value, Is.Null);
    }

    [Test]
    public async Task GetSecretValueAsync_logs_warning_when_unsuccessful()
    {
        var vault = _fixture.Create<string>();
        var secret = _fixture.Create<string>();
        _context.WithForbiddenResponse();
        await _context.Sut.GetSecretValueAsync(vault, secret);
        _context.AssertWarningLogged($"Failed GET request for /secrets/vaults/{vault}. Response Forbidden.");
    }

    [Test]
    public void GetSecretValueAsync_guards_against_missing_vault_argument()
    {
        var secret = _fixture.Create<string>();
        Assert.That(async () => await _context.Sut.GetSecretValueAsync(string.Empty, secret), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input vault was empty. (Parameter 'vault')"));
    }

    [Test]
    public void GetSecretValueAsync_guards_against_missing_secret_argument()
    {
        var vault = _fixture.Create<string>();
        Assert.That(async () => await _context.Sut.GetSecretValueAsync(vault, string.Empty), Throws.TypeOf<ArgumentException>().With.Property("Message").EqualTo("Required input secret was empty. (Parameter 'secret')"));
    }
}
