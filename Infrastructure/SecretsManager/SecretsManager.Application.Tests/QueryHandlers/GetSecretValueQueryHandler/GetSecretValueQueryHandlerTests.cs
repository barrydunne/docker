using SecretsManager.Application.Queries.GetSecretValue;

namespace SecretsManager.Application.Tests.QueryHandlers.GetSecretValueQueryHandler;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetSecretValueQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly GetSecretValueQueryHandlerTestsContext _context = new();

    [Test]
    public async Task GetSecretValueQueryHandler_returns_secrets_when_exist()
    {
        var vault = _fixture.Create<string>();
        var secrets = _fixture.Create<Dictionary<string, string>>();
        _context.WithVaultSecrets(vault, secrets);
        // Pick a random secret
        var secret = secrets.Keys.OrderBy(_ => Random.Shared.NextDouble()).First();
        var query = new GetSecretValueQuery(vault, secret);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        Assert.That(result.Value, Is.EqualTo(secrets[secret]));
    }

    [Test]
    public async Task GetSecretValueQueryHandler_returns_null_when_not_exists()
    {
        var vault = _fixture.Create<string>();
        var secret = _fixture.Create<string>();
        var query = new GetSecretValueQuery(vault, secret);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task GetSecretValueQueryHandler_returns_null_on_exception()
    {
        var vault = _fixture.Create<string>();
        var secret = _fixture.Create<string>();
        var query = new GetSecretValueQuery(vault, secret);
        _context.WithException();
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        Assert.That(result.Value, Is.Null);
        Assert.That(result.IsSuccess, Is.True);
    }
}
