using SecretsManager.Application.Queries.GetSecretVaults;

namespace SecretsManager.Application.Tests.QueryHandlers.GetSecretVaultsQueryHandler;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetSecretVaultsQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly GetSecretVaultsQueryHandlerTestsContext _context = new();

    [Test]
    public async Task GetSecretVaultsQueryHandler_returns_secrets_when_exist()
    {
        var vaults = _fixture.CreateMany<string>();
        foreach (var vault in vaults) 
            _context.WithVaultSecrets(vault, _fixture.Create<Dictionary<string, string>>());
        var query = new GetSecretVaultsQuery();
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        result.Value.ShouldBe(vaults);
    }

    [Test]
    public async Task GetSecretVaultsQueryHandler_returns_new_when_not_exists()
    {
        var query = new GetSecretVaultsQuery();
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        result.Value.ShouldBeEmpty();
    }

    [Test]
    public async Task GetSecretVaultsQueryHandler_returns_new_on_exception()
    {
        var query = new GetSecretVaultsQuery();
        _context.WithException();
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        result.Value.ShouldBeEmpty();
    }
}
