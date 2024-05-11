using Microservices.Shared.CloudSecrets;

namespace Microservices.Shared.Mocks;

/// <summary>
/// Provides in-memory secrets manager with instance scope.
/// </summary>
public class MockCloudSecrets : ICloudSecrets
{
    private readonly Dictionary<string, Dictionary<string, string>> _vaults;

    public MockCloudSecrets() => _vaults = new();

    public Task<Dictionary<string, string>> GetSecretsAsync(string vault, CancellationToken cancellationToken = default)
        => Task.FromResult(GetSecrets(vault));

    public Task<string?> GetSecretValueAsync(string vault, string secret, CancellationToken cancellationToken = default)
    {
        var secrets = GetSecrets(vault);
        return Task.FromResult(secrets.ContainsKey(secret) ? secrets[secret] : null);
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

    private Dictionary<string, string> GetSecrets(string vault) => _vaults.ContainsKey(vault) ? _vaults[vault] : new();
}
