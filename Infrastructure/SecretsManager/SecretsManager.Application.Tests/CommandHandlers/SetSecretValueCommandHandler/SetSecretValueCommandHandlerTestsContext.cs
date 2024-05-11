using Microservices.Shared.Mocks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SecretsManager.Application.Tests.CommandHandlers.SetSecretValueCommandHandler;

internal class SetSecretValueCommandHandlerTestsContext
{
    private readonly IRedisDatabase _mockRedisDatabase;
    private readonly MockLogger<Commands.SetSecretValue.SetSecretValueCommandHandler> _mockLogger;

    internal Dictionary<string, Dictionary<string, string>> Vaults { get; }

    internal Commands.SetSecretValue.SetSecretValueCommandHandler Sut { get; }

    public SetSecretValueCommandHandlerTestsContext()
    {
        _mockRedisDatabase = Substitute.For<IRedisDatabase>();
        _mockRedisDatabase
            .GetAsync<Dictionary<string, string>?>(Arg.Any<string>(), Arg.Any<CommandFlags>())
            .Returns(callInfo => GetVault(callInfo.ArgAt<string>(0)));
        _mockRedisDatabase
            .AddAsync<Arg.AnyType>(Arg.Any<string>(), Arg.Any<Arg.AnyType>(), Arg.Any<When>(), Arg.Any<CommandFlags>(), Arg.Any<HashSet<string>?>())
            .Returns(callInfo => SetValue(callInfo.ArgAt<string>(0), callInfo.ArgAt<object>(1)));

        _mockLogger = new();

        Vaults = new();
        Sut = new(_mockRedisDatabase, _mockLogger);
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
        _mockRedisDatabase
            .GetAsync<Dictionary<string, string>?>(Arg.Any<string>(), Arg.Any<CommandFlags>())
            .Throws(new ApplicationException());
        return this;
    }
}
