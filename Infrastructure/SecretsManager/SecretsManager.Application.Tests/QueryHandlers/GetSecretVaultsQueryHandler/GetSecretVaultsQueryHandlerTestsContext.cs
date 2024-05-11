using Microservices.Shared.Mocks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SecretsManager.Application.Tests.QueryHandlers.GetSecretVaultsQueryHandler;

internal class GetSecretVaultsQueryHandlerTestsContext
{
    private readonly IRedisDatabase _mockRedisDatabase;
    private readonly MockLogger<Queries.GetSecretVaults.GetSecretVaultsQueryHandler> _mockLogger;

    internal Dictionary<string, Dictionary<string, string>> Vaults { get; }

    internal Queries.GetSecretVaults.GetSecretVaultsQueryHandler Sut { get; }

    public GetSecretVaultsQueryHandlerTestsContext()
    {
        _mockRedisDatabase = Substitute.For<IRedisDatabase>();
        _mockRedisDatabase
            .SearchKeysAsync("*".ToSecretVaultName())
            .Returns(callInfo => GetVaultNames());

        _mockLogger = new();

        Vaults = new();
        Sut = new(_mockRedisDatabase, _mockLogger);
    }

    private IEnumerable<string> GetVaultNames() => Vaults.Keys.ToArray();

    internal GetSecretVaultsQueryHandlerTestsContext WithVaultSecrets(string vault, Dictionary<string, string> secrets)
    {
        Vaults[vault.ToSecretVaultName()] = secrets;
        return this;
    }

    internal GetSecretVaultsQueryHandlerTestsContext WithException()
    {
        _mockRedisDatabase
            .SearchKeysAsync("*".ToSecretVaultName())
            .Throws(new ApplicationException());
        return this;
    }
}
