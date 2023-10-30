using AspNet.KickStarter.CQRS.Abstractions.Queries;

namespace SecretsManager.Logic.Queries
{
    /// <summary>
    /// Get a single secret value from a vault.
    /// </summary>
    /// <param name="Vault">The vault name.</param>
    /// <param name="Secret">The secret name.</param>
    public record GetSecretValueQuery(string Vault, string Secret) : IQuery<string?>;
}
