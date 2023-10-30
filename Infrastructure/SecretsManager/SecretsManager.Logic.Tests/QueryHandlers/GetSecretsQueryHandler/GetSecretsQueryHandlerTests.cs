using SecretsManager.Logic.Queries;

namespace SecretsManager.Logic.Tests.QueryHandlers.GetSecretsQueryHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "QueryHandlers")]
    internal class GetSecretsQueryHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly GetSecretsQueryHandlerTestsContext _context = new();

        [Test]
        public void GetSecretsQueryHandler_guards_against_empty_vault_property()
        {
            var query = new GetSecretsQuery(string.Empty);
            Assert.That(async () => await _context.Sut.Handle(query, CancellationToken.None), Throws.InstanceOf<ArgumentException>().With.Property("ParamName").EqualTo(nameof(GetSecretsQuery.Vault)));
        }

        [Test]
        public void GetSecretsQueryHandler_guards_against_null_query()
            => Assert.That(async () => await _context.Sut.Handle(null!, CancellationToken.None), Throws.InstanceOf<ArgumentException>().With.Property("ParamName").EqualTo(nameof(GetSecretsQuery.Vault)));

        [Test]
        public async Task GetSecretsQueryHandler_returns_secrets_when_exist()
        {
            var vault = _fixture.Create<string>();
            var secrets = _fixture.Create<Dictionary<string, string>>();
            _context.WithVaultSecrets(vault, secrets);
            var query = new GetSecretsQuery(vault);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.Keys.ToArray(), Is.EquivalentTo(secrets.Keys.ToArray()));
        }

        [Test]
        public async Task GetSecretsQueryHandler_returns_new_when_not_exists()
        {
            var vault = _fixture.Create<string>();
            var query = new GetSecretsQuery(vault);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.Keys.ToArray(), Is.Empty);
        }

        [Test]
        public async Task GetSecretsQueryHandler_returns_new_on_exception()
        {
            var vault = _fixture.Create<string>();
            var query = new GetSecretsQuery(vault);
            _context.WithException();
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result.Keys.ToArray(), Is.Empty);
        }
    }
}
