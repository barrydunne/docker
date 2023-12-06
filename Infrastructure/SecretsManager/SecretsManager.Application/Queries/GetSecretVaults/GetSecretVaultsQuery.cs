using AspNet.KickStarter.CQRS.Abstractions.Queries;

namespace SecretsManager.Application.Queries.GetSecretVaults;

/// <summary>
/// Get the names of all secret vaults.
/// </summary>
public class GetSecretVaultsQuery : IQuery<string[]> { }
