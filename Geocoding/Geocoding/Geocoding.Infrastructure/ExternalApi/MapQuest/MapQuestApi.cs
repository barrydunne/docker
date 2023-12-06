using Geocoding.Application;
using Geocoding.Application.ExternalApi;
using Geocoding.Infrastructure.ExternalApi.MapQuest.MapQuestModels;
using Microservices.Shared.CloudSecrets;
using Microservices.Shared.Events;
using Microservices.Shared.RestSharpFactory;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Diagnostics.CodeAnalysis;

namespace Geocoding.Infrastructure.ExternalApi.MapQuest;

/* This requires the following secret to be added
 * Vault:  api.keys
 * Secret: geocoding.mapquest
 *
 * For example:
 * POST http://localhost:10083/secrets/vaults/api.keys/geocoding.mapquest
 * Content-Type: text/plain
 *
 * MY_API_KEY
 */

/// <summary>
/// Uses MapQuest geocoding API.
/// </summary>
public class MapQuestApi : IExternalApi
{
    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "Fixed domain that client should not configure")]
    private const string _baseUrl = "https://www.mapquestapi.com";

    private readonly ICloudSecrets _secrets;
    private readonly IRestSharpClientFactory _restSharpFactory;
    private readonly ILogger _logger;
    private readonly AsyncLazy<string?> _lazyApiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapQuestApi"/> class.
    /// </summary>
    /// <param name="secrets">The secrets that contain the Google API Key.</param>
    /// <param name="restSharpFactory">The factory to create IRestClient instances.</param>
    /// <param name="logger">The logger to write to.</param>
    public MapQuestApi(ICloudSecrets secrets, IRestSharpClientFactory restSharpFactory, ILogger<MapQuestApi> logger)
    {
        _secrets = secrets;
        _restSharpFactory = restSharpFactory;
        _logger = logger;
        _lazyApiKey = new(async () => await _secrets.GetSecretValueAsync("api.keys", "geocoding.mapquest"));
    }

    /// <inheritdoc/>
    public async Task<Coordinates> GetCoordinatesAsync(string address, Guid correlationId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting MapQuest API Key from secrets. [{CorrelationId}]", correlationId);
            var apiKey = await _lazyApiKey;
            _logger.LogDebug("Got MapQuest API Key '{ApiKey}' from secrets. [{CorrelationId}]", apiKey, correlationId);

            var resource = $"/geocoding/v1/address?key={apiKey}";
            var content = new GeocodingRequest { Location = address };
            var request = new RestRequest(resource).AddBody(content);

            _logger.LogDebug("Creating RestClient for MapQuest. [{CorrelationId}]", correlationId);
            using var client = _restSharpFactory.CreateRestClient(new RestClientOptions(_baseUrl) { FailOnDeserializationError = true });

            _logger.LogDebug("Making GET request for {Address}. [{CorrelationId}]", address, correlationId);
            var response = await client.ExecutePostAsync<GeocodingResponse>(request, cancellationToken);
            if (response.IsSuccessful)
                _logger.LogDebug("Successful GET request. Response {Status}.", response.StatusCode);
            else
                _logger.LogWarning("Failed GET request. Response {Status}.", response.StatusCode);

            var results = response.Data?.Results;
            if (results?.Count > 0)
            {
                var result = results.FirstOrDefault(_ => _.Locations is not null);
                if (result is not null)
                {
                    var location = result.Locations!.FirstOrDefault(_ => _.LatLng?.Lat is not null && _.LatLng.Lng is not null);
                    if (location is not null)
                        return new Coordinates((decimal)location.LatLng!.Lat!.Value, (decimal)location.LatLng!.Lng!.Value);
                }
            }

            // Not using the following line to avoid additional branch code coverage requirements for tests
            //     if (response.Data?.Info?.Messages is not null)
            if (response.Data is not null && response.Data.Info is not null && response.Data.Info.Messages is not null)
            {
                foreach (var message in response.Data.Info.Messages)
                    _logger.LogWarning("MapQuest message: {Message}. [{CorrelationId}]", message, correlationId);
            }
            throw new GeocodingException("No geocoding result obtained from MapQuest.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to geocode '{Address}'. [{CorrelationId}]", address, correlationId);
            if (ex is GeocodingException)
                throw;
            throw new GeocodingException(ex.Message, ex);
        }
    }
}
