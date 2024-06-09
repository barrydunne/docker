using Ardalis.GuardClauses;
using Microservices.Shared.RestSharpFactory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Diagnostics;

namespace Microservices.Shared.CloudSecrets.SecretsManager;

/// <inheritdoc/>
public class SecretsManagerSecrets : ICloudSecrets
{
    private static readonly TimeSpan _cacheTTL = TimeSpan.FromSeconds(5);
    private static readonly Lazy<MemoryCache> _lazyCache = new(() => new MemoryCache(new MemoryCacheOptions()));

    private readonly SecretsManagerOptions _options;
    private readonly IRestSharpClientFactory _restSharpFactory;
    private readonly IRestSharpResiliencePipeline _restSharpResiliencePipeline;
    private readonly ILogger _logger;

    private static readonly ActivitySource _activitySource = new(CloudSecrets.ActivitySourceName);

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretsManagerSecrets"/> class.
    /// </summary>
    /// <param name="options">The connection options.</param>
    /// <param name="restSharpFactory">The factory to create IRestClient instances.</param>
    /// <param name="restSharpResiliencePipeline">The resilient pipeline to use when making requests.</param>
    /// <param name="logger">The logger to write to.</param>
    public SecretsManagerSecrets(IOptions<SecretsManagerOptions> options, IRestSharpClientFactory restSharpFactory, IRestSharpResiliencePipeline restSharpResiliencePipeline, ILogger<SecretsManagerSecrets> logger)
    {
        _options = options.Value;
        _restSharpFactory = restSharpFactory;
        _restSharpResiliencePipeline = restSharpResiliencePipeline;
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
        using var activity = _activitySource.StartActivity("SecretsManager Get", ActivityKind.Client);
        Guard.Against.NullOrEmpty(vault, nameof(vault));

        if (Cache.TryGetValue<Dictionary<string, string>>(vault, out var secrets) && (secrets is not null))
            return secrets;

        secrets = await GetAsync<Dictionary<string, string>>($"/secrets/vaults/{vault}", cancellationToken);
        Cache.Set(vault, secrets, _cacheTTL);
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

    private async Task<T?> GetAsync<T>(string resource, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Creating RestClient for {BaseUrl}", _options.BaseUrl);
            using var client = _restSharpFactory.CreateRestClient(new RestClientOptions(_options.BaseUrl) { FailOnDeserializationError = true });

            _logger.LogDebug("Making GET request for {Resource}", resource);
            var response = await client.ExecuteGetAsync<T>(new RestRequest(resource), _restSharpResiliencePipeline, cancellationToken);
            if (response.IsSuccessful)
                _logger.LogDebug("Successful GET request for {Resource}. Response {Status}.", resource, response.StatusCode);
            else
                _logger.LogWarning("Failed GET request for {Resource}. Response {Status}.", resource, response.StatusCode);
            return response.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed GET request for {Resource}.", resource);
        }
        return default;
    }
}
