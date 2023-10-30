using Imaging.ExternalService.BingModels;
using Microservices.Shared.CloudSecrets;
using Microservices.Shared.Events;
using Microservices.Shared.RestSharpFactory;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Imaging.ExternalService
{
    /* This requires the following secret to be added
     * Vault:  api.keys
     * Secret: imaging.bing
     *
     * For example:
     * POST http://localhost:10083/secrets/vaults/api.keys/imaging.bing
     * Content-Type: text/plain
     *
     * MY_API_KEY
     */

    /// <summary>
    /// Uses Bing image API.
    /// </summary>
    public class Bing : IExternalService
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "Fixed domain that client should not configure")]
        private const string _baseUrl = "https://api.bing.microsoft.com";

        private readonly ICloudSecrets _secrets;
        private readonly IRestSharpClientFactory _restSharpFactory;
        private readonly ILogger _logger;
        private readonly AsyncLazy<string?> _lazyApiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bing"/> class.
        /// </summary>
        /// <param name="secrets">The secrets that contain the Google API Key.</param>
        /// <param name="restSharpFactory">The factory to create IRestClient instances.</param>
        /// <param name="logger">The logger to write to.</param>
        public Bing(ICloudSecrets secrets, IRestSharpClientFactory restSharpFactory, ILogger<Bing> logger)
        {
            _secrets = secrets;
            _restSharpFactory = restSharpFactory;
            _logger = logger;
            _lazyApiKey = new(async () => await _secrets.GetSecretValueAsync("api.keys", "imaging.bing"));
        }

        /// <inheritdoc/>
        public async Task<string?> GetImageUrlAsync(string address, Coordinates coordinates, Guid correlationId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Getting Bing API Key from secrets. [{CorrelationId}]", correlationId);
                var apiKey = await _lazyApiKey;
                _logger.LogDebug("Got Bing API Key '{ApiKey}' from secrets. [{CorrelationId}]", apiKey, correlationId);

                var resource = $"/v7.0/images/search?q={Uri.EscapeDataString(address)}&safeSearch=Moderate&color=ColorOnly&imageType=Photo";
                var request = new RestRequest(resource);

                _logger.LogDebug("Creating RestClient for Bing. [{CorrelationId}]", correlationId);
                using var client = _restSharpFactory.CreateRestClient(new RestClientOptions(_baseUrl) { FailOnDeserializationError = true });
                client.AddDefaultHeader("Ocp-Apim-Subscription-Key", apiKey!);

                _logger.LogDebug("Making GET request for images. [{CorrelationId}]", correlationId);
                var response = await client.ExecuteGetAsync<ImagesResponse>(request, cancellationToken);
                if (response.IsSuccessful)
                    _logger.LogDebug("Successful GET request. Response {Status}.", response.StatusCode);
                else
                    _logger.LogWarning("Failed GET request. Response {Status}.", response.StatusCode);

                var url = response.Data?.Value?.FirstOrDefault()?.ThumbnailUrl;
                if (!string.IsNullOrWhiteSpace(url))
                    return url;

                // Not using the following line to avoid additional branch code coverage requirements for tests
                //     if (response.Data?.Info?.Messages is not null)
                if ((response.Data is not null) && (response.Data.Errors is not null))
                {
                    foreach (var error in response.Data.Errors)
                        _logger.LogWarning("Bing message: {Message}. [{CorrelationId}]", error.Message, correlationId);
                }
                throw new ImagingException("No image result obtained from Bing.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get images for {Address} and {Coordinates}. [{CorrelationId}]", address, coordinates, correlationId);
                if (ex is ImagingException)
                    throw;
                throw new ImagingException(ex.Message, ex);
            }
        }
    }
}
