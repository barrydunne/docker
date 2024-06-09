using AutoFixture;

namespace Microservices.Shared.CloudSecrets.Aws.IntegrationTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "AwsSecrets")]
public class AwsSecretsTests : IDisposable
{
    private readonly AwsSecretsTestsContext _context = new();
    private readonly Fixture _fixture = new();
    private bool _disposedValue;

    [SetUp]
    public void Setup() => AwsSecrets.ClearCache();

    [Test]
    public async Task GetSecretsAsync_returns_secrets_when_exist()
    {
        var secrets = await _context.Sut.GetSecretsAsync(_context.KnownVault);
        Assert.That(secrets, Is.Not.Null.Or.Empty);
    }

    [Test]
    public async Task GetSecretsAsync_returns_empty_when_does_not_exist()
    {
        var vault = _fixture.Create<string>();
        var secrets = await _context.Sut.GetSecretsAsync(vault);
        Assert.That(secrets, Is.Empty);
    }

    [Test]
    public async Task GetSecretValueAsync_returns_secrets_when_exist()
    {
        var value = await _context.Sut.GetSecretValueAsync(_context.KnownVault, _context.KnownSecret);
        Assert.That(value, Is.EqualTo(_context.KnownValue));
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_vault_does_not_exist()
    {
        var vault = _fixture.Create<string>();
        var value = await _context.Sut.GetSecretValueAsync(vault, _context.KnownSecret);
        Assert.That(value, Is.Null);
    }

    [Test]
    public async Task GetSecretValueAsync_returns_null_when_secret_does_not_exist()
    {
        var secret = _fixture.Create<string>();
        var value = await _context.Sut.GetSecretValueAsync(_context.KnownVault, secret);
        Assert.That(value, Is.Null);
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
