using AutoFixture;

namespace Microservices.Shared.CloudSecrets.SecretsManager.IntegrationTests
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "SecretsManager")]
    public class SecretsManagerSecretsTests
    {
        private readonly SecretsManagerSecretsTestsContext _context = new();
        private readonly Fixture _fixture = new();

        private const string _vault = "infrastructure";
        private const string _secret = "rabbit.user";
        private const string _value = "admin";

        [SetUp]
        public void Setup() => SecretsManagerSecrets.ClearCache();

        [Test]
        public async Task GetSecretsAsync_returns_secrets_when_exist()
        {
            var secrets = await _context.Sut.GetSecretsAsync(_vault);
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
        public async Task GetSecretsAsync_returns_empty_when_forbidden()
        {
            _context.WithForbiddenResponse();
            var secrets = await _context.Sut.GetSecretsAsync(_vault);
            Assert.That(secrets, Is.Empty);
        }

        [Test]
        public async Task GetSecretsAsync_returns_empty_when_problem()
        {
            _context.WithProblemResponse();
            var secrets = await _context.Sut.GetSecretsAsync(_vault);
            Assert.That(secrets, Is.Empty);
        }

        [Test]
        public async Task GetSecretsAsync_returns_empty_when_unknown_base_url()
        {
            _context.WithUnknownHost();
            var secrets = await _context.Sut.GetSecretsAsync(_vault);
            Assert.That(secrets, Is.Empty);
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
            Assert.That(value, Is.EqualTo(_value));
        }

        [Test]
        public async Task GetSecretValueAsync_returns_null_when_vault_does_not_exist()
        {
            var vault = _fixture.Create<string>();
            var value = await _context.Sut.GetSecretValueAsync(vault, _secret);
            Assert.That(value, Is.Null);
        }

        [Test]
        public async Task GetSecretValueAsync_returns_null_when_secret_does_not_exist()
        {
            var secret = _fixture.Create<string>();
            var value = await _context.Sut.GetSecretValueAsync(_vault, secret);
            Assert.That(value, Is.Null);
        }

        [Test]
        public async Task GetSecretValueAsync_returns_null_when_forbidden()
        {
            _context.WithForbiddenResponse();
            var value = await _context.Sut.GetSecretValueAsync(_vault, _secret);
            Assert.That(value, Is.Null);
        }

        [Test]
        public async Task GetSecretValueAsync_returns_null_when_problem()
        {
            _context.WithProblemResponse();
            var value = await _context.Sut.GetSecretValueAsync(_vault, _secret);
            Assert.That(value, Is.Null);
        }

        [Test]
        public async Task GetSecretValueAsync_returns_null_when_unknown_base_url()
        {
            _context.WithUnknownHost();
            var value = await _context.Sut.GetSecretValueAsync(_vault, _secret);
            Assert.That(value, Is.Null);
        }

        [Test]
        public async Task GetSecretValueAsync_logs_warning_when_unsuccessful()
        {
            _context.WithForbiddenResponse();
            await _context.Sut.GetSecretValueAsync(_vault, _secret);
            _context.AssertWarningLogged($"Failed GET request for /secrets/vaults/{_vault}. Response Forbidden.");
        }
    }
}
