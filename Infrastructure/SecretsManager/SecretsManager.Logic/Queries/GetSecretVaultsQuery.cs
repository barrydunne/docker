using AspNet.KickStarter.CQRS.Abstractions.Queries;

namespace SecretsManager.Logic.Queries
{
    /// <summary>
    /// Get the names of all secret vaults.
    /// </summary>
    public class GetSecretVaultsQuery : IQuery<string[]> { }
}
