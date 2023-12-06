using Microservices.Shared.Mocks;
using Moq;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SecretsManager.Application.Tests.QueryHandlers.GetSecretVaultsQueryHandler;

internal class GetSecretVaultsQueryHandlerTestsContext
{
    private readonly Mock<IRedisDatabase> _mockRedisDatabase;
    private readonly MockLogger<Queries.GetSecretVaults.GetSecretVaultsQueryHandler> _mockLogger;

    internal Dictionary<string, Dictionary<string, string>> Vaults { get; }

    internal Queries.GetSecretVaults.GetSecretVaultsQueryHandler Sut { get; }

    public GetSecretVaultsQueryHandlerTestsContext()
    {
        _mockRedisDatabase = new(MockBehavior.Strict);
        _mockRedisDatabase.Setup(_ => _.SearchKeysAsync("*".ToSecretVaultName()))
            .ReturnsAsync((string key) => GetVaultNames());

        _mockLogger = new();

        Vaults = new();
        Sut = new(_mockRedisDatabase.Object, _mockLogger.Object);
    }

    private IEnumerable<string> GetVaultNames() => Vaults.Keys.ToArray();

    internal GetSecretVaultsQueryHandlerTestsContext WithVaultSecrets(string vault, Dictionary<string, string> secrets)
    {
        Vaults[vault.ToSecretVaultName()] = secrets;
        return this;
    }

    internal GetSecretVaultsQueryHandlerTestsContext WithException()
    {
        _mockRedisDatabase.Setup(_ => _.SearchKeysAsync("*".ToSecretVaultName()))
            .Throws(() => new ApplicationException());
        return this;
    }
}
