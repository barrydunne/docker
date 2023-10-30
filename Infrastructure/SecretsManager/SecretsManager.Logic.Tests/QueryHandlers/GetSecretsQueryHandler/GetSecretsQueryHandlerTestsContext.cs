using Microservices.Shared.Mocks;
using Moq;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SecretsManager.Logic.Tests.QueryHandlers.GetSecretsQueryHandler
{
    internal class GetSecretsQueryHandlerTestsContext
    {
        private readonly Mock<IRedisDatabase> _mockRedisDatabase;
        private readonly MockLogger<SecretsManager.Logic.QueryHandlers.GetSecretsQueryHandler> _mockLogger;

        internal Dictionary<string, Dictionary<string, string>> Vaults { get; }

        internal SecretsManager.Logic.QueryHandlers.GetSecretsQueryHandler Sut { get; }

        public GetSecretsQueryHandlerTestsContext()
        {
            _mockRedisDatabase = new(MockBehavior.Strict);
            _mockRedisDatabase.Setup(_ => _.GetAsync<Dictionary<string, string>?>(It.IsAny<string>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync((string key, CommandFlags _) => GetVault(key));

            _mockLogger = new();

            Vaults = new();            
            Sut = new(_mockRedisDatabase.Object, _mockLogger.Object);
        }

        private Dictionary<string, string>? GetVault(string key) => Vaults.ContainsKey(key) ? Vaults[key] : null;

        internal GetSecretsQueryHandlerTestsContext WithVaultSecrets(string vault, Dictionary<string, string> secrets)
        {
            Vaults[vault.ToSecretVaultName()] = secrets;
            return this;
        }

        internal GetSecretsQueryHandlerTestsContext WithException()
        {
            _mockRedisDatabase.Setup(_ => _.GetAsync<Dictionary<string, string>?>(It.IsAny<string>(), It.IsAny<CommandFlags>()))
                .Throws(() => new ApplicationException());
            return this;
        }
    }
}
