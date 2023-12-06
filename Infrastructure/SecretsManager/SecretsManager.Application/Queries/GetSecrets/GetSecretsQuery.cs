using AspNet.KickStarter.CQRS.Abstractions.Queries;

namespace SecretsManager.Application.Queries.GetSecrets;

/// <summary>
/// Get all secrets from a vault.
/// </summary>
/// <param name="Vault">The vault name.</param>
public record GetSecretsQuery(string Vault) : IQuery<Dictionary<string, string>>;
