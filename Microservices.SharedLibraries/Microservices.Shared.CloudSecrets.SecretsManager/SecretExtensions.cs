using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microservices.Shared.CloudSecrets.SecretsManager
{
    /// <summary>
    /// Provides extensions for AspNet dependency injection.
    /// </summary>
    public static class SecretExtensions
    {
        private static ServiceProvider? _serviceProvider = null;

        /// <summary>
        /// Add the SecretsManagerSecrets type as the ICloudSecrets service.
        /// </summary>
        /// <param name="builder">The builder to register the service with.</param>
        /// <param name="configSectionName">The name of the configuration section to use to create the required SecretsManagerOptions, defaults to "SecretsManagerOptions".</param>
        /// <returns>The original builder.</returns>
        public static IHostApplicationBuilder AddSecretsManagerSecrets(this IHostApplicationBuilder builder, string configSectionName = "SecretsManagerOptions")
        {
            builder.Services.AddTransient<ICloudSecrets, SecretsManagerSecrets>()
                   .Configure<SecretsManagerOptions>(builder.Configuration.GetSection(configSectionName));
            return builder;
        }

        /// <summary>
        /// Apply a secret value to the configuration.
        /// </summary>
        /// <param name="config">The configuration section to modify.</param>
        /// <param name="builder">The builder that has the ICloudSecrets service already registered.</param>
        /// <param name="key">The configuration key to modify.</param>
        /// <param name="vault">The secret vault name.</param>
        /// <param name="secret">The secret name.</param>
        /// <returns>The original configuration section.</returns>
        public static IConfigurationSection ApplySecret(this IConfigurationSection config, IHostApplicationBuilder builder, string key, string vault, string secret)
        {
            _serviceProvider ??= builder.Services.BuildServiceProvider();
            var cloudSecrets = _serviceProvider.GetService<ICloudSecrets>();
            if (cloudSecrets is not null)
                config.GetSection(key).Value = cloudSecrets.GetSecretValueAsync(vault, secret).Result;
            return config;
        }
    }
}
