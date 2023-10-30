using AspNet.KickStarter.CQRS.Abstractions.Commands;

namespace SecretsManager.Logic.Commands
{
    /// <summary>
    /// Set a single secret value in a vault.
    /// </summary>
    /// <param name="Vault">The vault name.</param>
    /// <param name="Secret">The secret name.</param>
    /// <param name="Value">The secret value.</param>
    public record SetSecretValueCommand(string Vault, string Secret, string Value) : ICommand;
}
