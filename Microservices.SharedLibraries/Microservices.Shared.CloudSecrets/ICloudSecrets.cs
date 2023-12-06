namespace Microservices.Shared.CloudSecrets;

/// <summary>
/// Provides the ability to retrieve secrets from a secure vault.
/// </summary>
public interface ICloudSecrets
{
    /// <summary>
    /// Get the secrets contained in a vault.
    /// </summary>
    /// <param name="vault">The vault name.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The secrets contained in the specified vault, or an empty Dictionary if not found.</returns>
    Task<Dictionary<string, string>> GetSecretsAsync(string vault, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a single secret value from a vault.
    /// </summary>
    /// <param name="vault">The vault name.</param>
    /// <param name="secret">The name of the secret to get the value for.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The value of the secret from the specified vault, or null if the vault or secret are not found.</returns>
    Task<string?> GetSecretValueAsync(string vault, string secret, CancellationToken cancellationToken = default);
}
