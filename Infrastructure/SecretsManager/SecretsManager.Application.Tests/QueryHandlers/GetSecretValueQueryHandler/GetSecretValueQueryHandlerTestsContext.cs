using Microservices.Shared.Mocks;
using Moq;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SecretsManager.Application.Tests.QueryHandlers.GetSecretValueQueryHandler;

internal class GetSecretValueQueryHandlerTestsContext
{
    private readonly Mock<IRedisDatabase> _mockRedisDatabase;
    private readonly MockLogger<Queries.GetSecretValue.GetSecretValueQueryHandler> _mockLogger;

    internal Dictionary<string, Dictionary<string, string>> Vaults { get; }

    internal Queries.GetSecretValue.GetSecretValueQueryHandler Sut { get; }

    public GetSecretValueQueryHandlerTestsContext()
    {
        _mockRedisDatabase = new(MockBehavior.Strict);
        _mockRedisDatabase.Setup(_ => _.GetAsync<Dictionary<string, string>?>(It.IsAny<string>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((string key, CommandFlags _) => GetVault(key));

        _mockLogger = new();

        Vaults = new();
        Sut = new(_mockRedisDatabase.Object, _mockLogger.Object);
    }

    private Dictionary<string, string>? GetVault(string key) => Vaults.ContainsKey(key) ? Vaults[key] : null;

    internal GetSecretValueQueryHandlerTestsContext WithVaultSecrets(string vault, Dictionary<string, string> secrets)
    {
        Vaults[vault.ToSecretVaultName()] = secrets;
        return this;
    }

    internal GetSecretValueQueryHandlerTestsContext WithException()
    {
        _mockRedisDatabase.Setup(_ => _.GetAsync<Dictionary<string, string>?>(It.IsAny<string>(), It.IsAny<CommandFlags>()))
            .Throws(() => new ApplicationException());
        return this;
    }
}
