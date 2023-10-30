using SecretsManager.Logic.Commands;

namespace SecretsManager.Logic.Tests.CommandHandlers.SetSecretValueCommandHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "CommandHandlers")]
    internal class SetSecretValueCommandHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly SetSecretValueCommandHandlerTestsContext _context = new();

        [Test]
        public void SetSecretValueCommandHandler_guards_against_missing_vault()
        {
            var command = new SetSecretValueCommand(string.Empty, _fixture.Create<string>(), _fixture.Create<string>());
            Assert.That(async () => await _context.Sut.Handle(command, CancellationToken.None), Throws.InstanceOf<ArgumentException>().With.Property("ParamName").EqualTo(nameof(SetSecretValueCommand.Vault)));
        }

        [Test]
        public void SetSecretValueCommandHandler_guards_against_missing_secret()
        {
            var command = new SetSecretValueCommand(_fixture.Create<string>(), string.Empty, _fixture.Create<string>());
            Assert.That(async () => await _context.Sut.Handle(command, CancellationToken.None), Throws.InstanceOf<ArgumentException>().With.Property("ParamName").EqualTo(nameof(SetSecretValueCommand.Secret)));
        }

        [Test]
        public void SetSecretValueCommandHandler_guards_against_null_command()
            => Assert.That(async () => await _context.Sut.Handle(null!, CancellationToken.None), Throws.InstanceOf<ArgumentException>().With.Property("ParamName").EqualTo(nameof(SetSecretValueCommand.Vault)));

        [Test]
        public async Task SetSecretValueCommandHandler_replaces_secret_when_exists()
        {
            var vault = _fixture.Create<string>();
            var secrets = _fixture.Create<Dictionary<string, string>>();
            _context.WithVaultSecrets(vault, secrets);
            // Pick a random secret
            var secret = secrets.Keys.OrderBy(_ => Random.Shared.NextDouble()).First();
            var value = _fixture.Create<string>();
            var command = new SetSecretValueCommand(vault, secret, value);
            await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(_context.Vaults[vault.ToSecretVaultName()][secret], Is.EqualTo(value));
        }

        [Test]
        public async Task SetSecretValueCommandHandler_returns_success_when_exists()
        {
            var vault = _fixture.Create<string>();
            var secrets = _fixture.Create<Dictionary<string, string>>();
            _context.WithVaultSecrets(vault, secrets);
            // Pick a random secret
            var secret = secrets.Keys.OrderBy(_ => Random.Shared.NextDouble()).First();
            var value = _fixture.Create<string>();
            var command = new SetSecretValueCommand(vault, secret, value);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task SetSecretValueCommandHandler_adds_secret_when_does_not_exist_in_vault()
        {
            var vault = _fixture.Create<string>();
            var secrets = _fixture.Create<Dictionary<string, string>>();
            _context.WithVaultSecrets(vault, secrets);
            // Pick a new secret
            var secret = _fixture.Create<string>();
            var value = _fixture.Create<string>();
            var command = new SetSecretValueCommand(vault, secret, value);
            await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(_context.Vaults[vault.ToSecretVaultName()][secret], Is.EqualTo(value));
        }

        [Test]
        public async Task SetSecretValueCommandHandler_returns_success_when_does_not_exist_in_vault()
        {
            var vault = _fixture.Create<string>();
            var secrets = _fixture.Create<Dictionary<string, string>>();
            _context.WithVaultSecrets(vault, secrets);
            // Pick a new secret
            var secret = _fixture.Create<string>();
            var value = _fixture.Create<string>();
            var command = new SetSecretValueCommand(vault, secret, value);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task SetSecretValueCommandHandler_adds_secret_when_vault_does_not_exist()
        {
            var vault = _fixture.Create<string>();
            var secret = _fixture.Create<string>();
            var value = _fixture.Create<string>();
            var command = new SetSecretValueCommand(vault, secret, value);
            await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(_context.Vaults[vault.ToSecretVaultName()][secret], Is.EqualTo(value));
        }

        [Test]
        public async Task SetSecretValueCommandHandler_returns_success_when_vault_does_not_exist()
        {
            var vault = _fixture.Create<string>();
            var secret = _fixture.Create<string>();
            var value = _fixture.Create<string>();
            var command = new SetSecretValueCommand(vault, secret, value);
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task SetSecretValueCommandHandler_returns_failure_on_exception()
        {
            var vault = _fixture.Create<string>();
            var secret = _fixture.Create<string>();
            var value = _fixture.Create<string>();
            var command = new SetSecretValueCommand(vault, secret, value);
            _context.WithException();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }
    }
}
