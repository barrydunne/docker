using Directions.Application;
using Directions.Application.ExternalApi;
using Directions.Infrastructure.ExternalApi.MapQuest.MapQuestModels;
using Microservices.Shared.CloudSecrets;
using Microservices.Shared.Events;
using Microservices.Shared.RestSharpFactory;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Diagnostics.CodeAnalysis;

namespace Directions.Infrastructure.ExternalApi.MapQuest;

/* This requires the following secret to be added
 * Vault:  api.keys
 * Secret: directions.mapquest
 *
 * For example:
 * POST http://localhost:10083/secrets/vaults/api.keys/directions.mapquest
 * Content-Type: text/plain
 *
 * MY_API_KEY
 */

/// <summary>
/// Uses MapQuest directions API.
/// </summary>
public class MapQuestApi : IExternalApi
{
    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "Fixed domain that client should not configure")]
    private const string _baseUrl = "https://www.mapquestapi.com";

    private readonly ICloudSecrets _secrets;
    private readonly IRestSharpClientFactory _restSharpFactory;
    private readonly IRestSharpResiliencePipeline _restSharpResiliencePipeline;
    private readonly ILogger _logger;
    private readonly AsyncLazy<string?> _lazyApiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapQuestApi"/> class.
    /// </summary>
    /// <param name="secrets">The secrets that contain the Google API Key.</param>
    /// <param name="restSharpFactory">The factory to create IRestClient instances.</param>
    /// <param name="restSharpResiliencePipeline">The resilient pipeline to use when making requests.</param>
    /// <param name="logger">The logger to write to.</param>
    public MapQuestApi(ICloudSecrets secrets, IRestSharpClientFactory restSharpFactory, IRestSharpResiliencePipeline restSharpResiliencePipeline, ILogger<MapQuestApi> logger)
    {
        _secrets = secrets;
        _restSharpFactory = restSharpFactory;
        _restSharpResiliencePipeline = restSharpResiliencePipeline;
        _logger = logger;
        _lazyApiKey = new(async () => await _secrets.GetSecretValueAsync("api.keys", "directions.mapquest"));
    }

    /// <inheritdoc/>
    public async Task<Microservices.Shared.Events.Directions> GetDirectionsAsync(Coordinates startingCoordinates, Coordinates destinationCoordinates, Guid correlationId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting MapQuest API Key from secrets. [{CorrelationId}]", correlationId);
            var apiKey = await _lazyApiKey;
            _logger.LogDebug("Got MapQuest API Key '{ApiKey}' from secrets. [{CorrelationId}]", apiKey, correlationId);

            var resource = $"/directions/v2/route?key={apiKey}";
            var content = new DirectionsRequest { Locations = new[] { $"{startingCoordinates.Latitude},{startingCoordinates.Longitude}", $"{destinationCoordinates.Latitude},{destinationCoordinates.Longitude}" } };
            var request = new RestRequest(resource).AddBody(content);

            _logger.LogDebug("Creating RestClient for MapQuest. [{CorrelationId}]", correlationId);
            using var client = _restSharpFactory.CreateRestClient(new RestClientOptions(_baseUrl) { FailOnDeserializationError = true });

            _logger.LogDebug("Making GET request for directions. [{CorrelationId}]", correlationId);
            var response = await client.ExecutePostAsync<DirectionsResponse>(request, _restSharpResiliencePipeline, cancellationToken);
            if (response.IsSuccessful)
                _logger.LogDebug("Successful GET request. Response {Status}.", response.StatusCode);
            else
                _logger.LogWarning("Failed GET request. Response {Status}.", response.StatusCode);

            var route = response.Data?.Route;
            var legs = route?.Legs?.Where(_ => _.Maneuvers is not null);
            var maneuvers = legs?.SelectMany(_ => _.Maneuvers!);
            if (maneuvers?.Any() == true)
            {
                var steps = maneuvers.Where(_ => _ is not null).Select(_ => new DirectionsStep(_.Narrative ?? string.Empty, _.Time, _.Distance)).ToArray();
                return new Microservices.Shared.Events.Directions(true, route!.RealTime, route.Distance, steps, null);
            }

            // Not using the following line to avoid additional branch code coverage requirements for tests
            //     if (response.Data?.Info?.Messages is not null)
            if (response.Data is not null && response.Data.Info is not null && response.Data.Info.Messages is not null)
            {
                foreach (var message in response.Data.Info.Messages)
                    _logger.LogWarning("MapQuest message: {Message}. [{CorrelationId}]", message, correlationId);
            }
            throw new DirectionsException("No directions result obtained from MapQuest.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get directions between {StartingCoordinates} and {DestinationCoordinates}. [{CorrelationId}]", startingCoordinates, destinationCoordinates, correlationId);
            if (ex is DirectionsException)
                throw;
            throw new DirectionsException(ex.Message, ex);
        }
    }
}
