using Microservices.Shared.Mocks;
using NSubstitute.ExceptionExtensions;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SecretsManager.Application.Tests.QueryHandlers.GetSecretValueQueryHandler;

internal class GetSecretValueQueryHandlerTestsContext
{
    private readonly IRedisDatabase _mockRedisDatabase;
    private readonly MockLogger<Queries.GetSecretValue.GetSecretValueQueryHandler> _mockLogger;

    internal Dictionary<string, Dictionary<string, string>> Vaults { get; }

    internal Queries.GetSecretValue.GetSecretValueQueryHandler Sut { get; }

    public GetSecretValueQueryHandlerTestsContext()
    {
        _mockRedisDatabase = Substitute.For<IRedisDatabase>();
        _mockRedisDatabase
            .GetAsync<Dictionary<string, string>?>(Arg.Any<string>(), Arg.Any<CommandFlags>())
            .Returns(callInfo => GetVault(callInfo.ArgAt<string>(0)));

        _mockLogger = new();

        Vaults = new();
        Sut = new(_mockRedisDatabase, _mockLogger);
    }

    private Dictionary<string, string>? GetVault(string key) => Vaults.ContainsKey(key) ? Vaults[key] : null;

    internal GetSecretValueQueryHandlerTestsContext WithVaultSecrets(string vault, Dictionary<string, string> secrets)
    {
        Vaults[vault.ToSecretVaultName()] = secrets;
        return this;
    }

    internal GetSecretValueQueryHandlerTestsContext WithException()
    {
        _mockRedisDatabase
            .GetAsync<Dictionary<string, string>?>(Arg.Any<string>(), Arg.Any<CommandFlags>())
            .Throws(new ApplicationException());
        return this;
    }
}
