namespace SecretsManager.Logic
{
    /// <summary>
    /// Provides constant values available for use throughout the application.
    /// </summary>
    internal static class Consts
    {
        /// <summary>
        /// The prefix used on vault names when stored in Redis.
        /// </summary>
        internal const string SecretVaultPrefix = "microservices.";

        /// <summary>
        /// The suffix used on vault names when stored in Redis.
        /// </summary>
        internal const string SecretVaultSuffix = ".secrets";
    }
}
