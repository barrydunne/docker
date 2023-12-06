using SecretsManager.Application.Queries.GetSecrets;

namespace SecretsManager.Application.Tests.QueryHandlers.GetSecretsQueryHandler;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetSecretsQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly GetSecretsQueryHandlerTestsContext _context = new();

    [Test]
    public async Task GetSecretsQueryHandler_returns_secrets_when_exist()
    {
        var vault = _fixture.Create<string>();
        var secrets = _fixture.Create<Dictionary<string, string>>();
        _context.WithVaultSecrets(vault, secrets);
        var query = new GetSecretsQuery(vault);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        Assert.That(result.Value.Keys.ToArray(), Is.EquivalentTo(secrets.Keys.ToArray()));
    }

    [Test]
    public async Task GetSecretsQueryHandler_returns_new_when_not_exists()
    {
        var vault = _fixture.Create<string>();
        var query = new GetSecretsQuery(vault);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        Assert.That(result.Value.Keys.ToArray(), Is.Empty);
    }

    [Test]
    public async Task GetSecretsQueryHandler_returns_new_on_exception()
    {
        var vault = _fixture.Create<string>();
        var query = new GetSecretsQuery(vault);
        _context.WithException();
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        Assert.That(result.Value.Keys.ToArray(), Is.Empty);
    }
}
