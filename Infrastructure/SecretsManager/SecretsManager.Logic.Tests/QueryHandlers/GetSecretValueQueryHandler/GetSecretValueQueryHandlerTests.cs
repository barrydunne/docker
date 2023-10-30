using SecretsManager.Logic.Queries;

namespace SecretsManager.Logic.Tests.QueryHandlers.GetSecretValueQueryHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "QueryHandlers")]
    internal class GetSecretValueQueryHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly GetSecretValueQueryHandlerTestsContext _context = new();

        [Test]
        public void GetSecretValueQueryHandler_guards_against_empty_vault()
        {
            var query = new GetSecretValueQuery(string.Empty, _fixture.Create<string>());
            Assert.That(async () => await _context.Sut.Handle(query, CancellationToken.None), Throws.InstanceOf<ArgumentException>().With.Property("ParamName").EqualTo(nameof(GetSecretValueQuery.Vault)));
        }

        [Test]
        public void GetSecretValueQueryHandler_guards_against_empty_secret()
        {
            var query = new GetSecretValueQuery(_fixture.Create<string>(), string.Empty);
            Assert.That(async () => await _context.Sut.Handle(query, CancellationToken.None), Throws.InstanceOf<ArgumentException>().With.Property("ParamName").EqualTo(nameof(GetSecretValueQuery.Secret)));
        }

        [Test]
        public void SetSecretValueCommandHandler_guards_against_null_query()
            => Assert.That(async () => await _context.Sut.Handle(null!, CancellationToken.None), Throws.InstanceOf<ArgumentException>().With.Property("ParamName").EqualTo(nameof(GetSecretValueQuery.Vault)));

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
            Assert.That(result, Is.EqualTo(secrets[secret]));
        }

        [Test]
        public async Task GetSecretValueQueryHandler_returns_null_when_not_exists()
        {
            var vault = _fixture.Create<string>();
            var secret = _fixture.Create<string>();
            var query = new GetSecretValueQuery(vault, secret);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetSecretValueQueryHandler_returns_null_on_exception()
        {
            var vault = _fixture.Create<string>();
            var secret = _fixture.Create<string>();
            var query = new GetSecretValueQuery(vault, secret);
            _context.WithException();
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result, Is.Null);
        }
    }
}
