using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microservices.Shared.CloudSecrets;

/// <summary>
/// Provides extensions for dependency injection.
/// </summary>
public static class ServiceExtensions
{
    private static ServiceProvider? _serviceProvider = null;

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
