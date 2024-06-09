using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Microservices.Shared.CloudSecrets.Aws;

/// <inheritdoc/>
public class AwsSecrets : ICloudSecrets
{
    private static readonly TimeSpan _cacheTTL = TimeSpan.FromSeconds(5);
    private static readonly Lazy<MemoryCache> _lazyCache = new(() => new MemoryCache(new MemoryCacheOptions()));

    private readonly IAmazonSecretsManager _amazonSecretsManager;
    private readonly ILogger _logger;

    private static readonly ActivitySource _activitySource = new(CloudSecrets.ActivitySourceName);

    /// <summary>
    /// Initializes a new instance of the <see cref="AwsSecrets"/> class.
    /// </summary>
    /// <param name="amazonSecretsManager">The Amazon SecretsManager client to use.</param>
    /// <param name="logger">The logger to write to.</param>
    public AwsSecrets(IAmazonSecretsManager amazonSecretsManager, ILogger<AwsSecrets> logger)
    {
        _amazonSecretsManager = amazonSecretsManager;
        _logger = logger;
    }

    private static MemoryCache Cache => _lazyCache.Value;

    /// <summary>
    /// May be used to clear any cached vaults to ensure the next request will use live data.
    /// </summary>
    public static void ClearCache() => Cache.Clear();

    /// <inheritdoc/>
    public async Task<Dictionary<string, string>> GetSecretsAsync(string vault, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity("AWS Get secrets", ActivityKind.Client);
        Guard.Against.NullOrEmpty(vault, nameof(vault));

        if (Cache.TryGetValue<Dictionary<string, string>>(vault, out var secrets) && (secrets is not null))
            return secrets;

        _logger.LogInformation("Requesting {Vault} secrets", vault);
        try
        {
            var request = new GetSecretValueRequest { SecretId = vault, VersionStage = "AWSCURRENT" };
            var response = await _amazonSecretsManager.GetSecretValueAsync(request, cancellationToken);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                var secret = response.SecretString ?? Encoding.UTF8.GetString(Convert.FromBase64String(await new StreamReader(response.SecretBinary).ReadToEndAsync()));
                _logger.LogInformation("Loaded {Vault} at version {Version} ({Created})", vault, response.VersionId, response.CreatedDate);

                secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(secret);
                Cache.Set(vault, secrets, _cacheTTL);
            }
            else
                _logger.LogWarning("Failed GET request for {Vault}. Response {StatusCode}.", vault, response.HttpStatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get {Vault} secrets", vault);
        }
        return secrets ?? new();
    }

    /// <inheritdoc/>
    public async Task<string?> GetSecretValueAsync(string vault, string secret, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(vault, nameof(vault));
        Guard.Against.NullOrEmpty(secret, nameof(secret));

        var secrets = await GetSecretsAsync(vault, cancellationToken);
        if (secrets?.TryGetValue(secret, out var value) == true)
            return value;
        return null;
    }
}
