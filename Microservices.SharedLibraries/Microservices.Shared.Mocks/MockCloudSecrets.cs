using Microservices.Shared.CloudSecrets;
using Moq;

namespace Microservices.Shared.Mocks
{
    /// <summary>
    /// Provides in-memory secrets manager with instance scope.
    /// </summary>
    public class MockCloudSecrets : Mock<ICloudSecrets>
    {
        private readonly Dictionary<string, Dictionary<string, string>> _vaults;

        public MockCloudSecrets() : base(MockBehavior.Strict)
        {
            _vaults = new();

            Setup(_ => _.GetSecretsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string vault, CancellationToken _) => GetSecrets(vault));

            Setup(_ => _.GetSecretValueAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string vault, string secret, CancellationToken _) => GetSecretVault(vault, secret));
        }

        public void WithSecretValue(string vault, string secret, string value)
        {
            var secrets = GetSecrets(vault);
            secrets[secret] = value;
            _vaults[vault] = secrets;
        }

        public void WithoutSecretValue(string vault, string secret)
        {
            var secrets = GetSecrets(vault);
            if (secrets.ContainsKey(secret))
            {
                secrets.Remove(secret);
                _vaults[vault] = secrets;
            }
        }

        public Dictionary<string, string> GetSecrets(string vault)
            => _vaults.ContainsKey(vault) ? _vaults[vault] : new();

        private string? GetSecretVault(string vault, string secret)
        {
            var secrets = GetSecrets(vault);
            return secrets.ContainsKey(secret) ? secrets[secret] : null;
        }
    }
}
