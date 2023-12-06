using SecretsManager.Application.Commands.SetSecretValue;

namespace SecretsManager.Application.Tests.CommandHandlers.SetSecretValueCommandHandler;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class SetSecretValueCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly SetSecretValueCommandHandlerTestsContext _context = new();

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
    public async Task SetSecretValueCommandHandler_returns_error_on_exception()
    {
        var vault = _fixture.Create<string>();
        var secret = _fixture.Create<string>();
        var value = _fixture.Create<string>();
        var command = new SetSecretValueCommand(vault, secret, value);
        _context.WithException();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }
}
