using Microservices.Shared.Mocks;
using Moq;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SecretsManager.Application.Tests.CommandHandlers.SetSecretValueCommandHandler;

internal class SetSecretValueCommandHandlerTestsContext
{
    private readonly Mock<IRedisDatabase> _mockRedisDatabase;
    private readonly MockLogger<Commands.SetSecretValue.SetSecretValueCommandHandler> _mockLogger;

    internal Dictionary<string, Dictionary<string, string>> Vaults { get; }

    internal Commands.SetSecretValue.SetSecretValueCommandHandler Sut { get; }

    public SetSecretValueCommandHandlerTestsContext()
    {
        _mockRedisDatabase = new(MockBehavior.Strict);
        _mockRedisDatabase.Setup(_ => _.GetAsync<Dictionary<string, string>?>(It.IsAny<string>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((string key, CommandFlags _) => GetVault(key));
        _mockRedisDatabase.Setup(_ => _.AddAsync<It.IsAnyType>(It.IsAny<string>(), It.IsAny<It.IsAnyType>(), It.IsAny<When>(), It.IsAny<CommandFlags>(), It.IsAny<HashSet<string>?>()))
            .ReturnsAsync((string key, object value, When _, CommandFlags _, HashSet<string>? _) => SetValue(key, value));

        _mockLogger = new();

        Vaults = new();
        Sut = new(_mockRedisDatabase.Object, _mockLogger.Object);
    }

    private bool SetValue(string key, object value)
    {
        Vaults[key] = (Dictionary<string, string>)value;
        return true;
    }

    private Dictionary<string, string>? GetVault(string key) => Vaults.ContainsKey(key) ? Vaults[key] : null;

    internal SetSecretValueCommandHandlerTestsContext WithVaultSecrets(string vault, Dictionary<string, string> secrets)
    {
        Vaults[vault.ToSecretVaultName()] = secrets;
        return this;
    }

    internal SetSecretValueCommandHandlerTestsContext WithException()
    {
        _mockRedisDatabase.Setup(_ => _.GetAsync<Dictionary<string, string>?>(It.IsAny<string>(), It.IsAny<CommandFlags>()))
            .Throws(() => new ApplicationException());
        return this;
    }
}
