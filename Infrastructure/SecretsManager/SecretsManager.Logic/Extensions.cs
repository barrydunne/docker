namespace SecretsManager.Logic
{
    /// <summary>
    /// Provides extension methods available for use throughout the application.
    /// </summary>
    internal static class Extensions
    {
        private static readonly int _secretVaultNameStart = Consts.SecretVaultPrefix.Length;
        private static readonly int _secretVaultNameRemoveLength = Consts.SecretVaultPrefix.Length + Consts.SecretVaultSuffix.Length;

        /// <summary>
        /// Convert a vault name to a Redis key.
        /// </summary>
        /// <param name="name">The vault name.</param>
        /// <returns>The Redis key.</returns>
        internal static string ToSecretVaultName(this string name) => $"{Consts.SecretVaultPrefix}{name.ToLowerInvariant()}{Consts.SecretVaultSuffix}";

        /// <summary>
        /// Convert a Redis key to a vault name.
        /// </summary>
        /// <param name="key">The Redis key.</param>
        /// <returns>The vault name.</returns>
        internal static string FromSecretVaultName(this string key) => key.Substring(_secretVaultNameStart, key.Length - _secretVaultNameRemoveLength);
    }
}
