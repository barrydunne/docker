using AspNet.KickStarter.CQRS.Abstractions.Queries;

namespace SecretsManager.Logic.Queries
{
    /// <summary>
    /// Get all secrets from a vault.
    /// </summary>
    /// <param name="Vault">The vault name.</param>
    public record GetSecretsQuery(string Vault) : IQuery<Dictionary<string, string>>;
}
