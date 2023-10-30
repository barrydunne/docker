using Microservices.Shared.Events;
using Microservices.Shared.RestSharpFactory;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.IO.Abstractions;
using System.Reflection;
using System.Text.Json;
using Weather.ExternalService.OpenMeteoModels;

namespace Weather.ExternalService
{
    /// <summary>
    /// Uses OpenMeteo weather API.
    /// </summary>
    public class OpenMeteo : IExternalService
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "Fixed domain that client should not configure")]
        private const string _baseUrl = "https://api.open-meteo.com";

        private readonly IFileSystem _fileSystem;
        private readonly IRestSharpClientFactory _restSharpFactory;
        private readonly ILogger _logger;
        private readonly Lazy<Dictionary<int, Dictionary<string, Dictionary<string, string>>>> _lazyWmoCodesData;
        private readonly Lazy<Dictionary<int, string>> _lazyDescriptions;
        private readonly Lazy<Dictionary<int, string>> _lazyImageUrls;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenMeteo"/> class.
        /// </summary>
        /// <param name="fileSystem">The testable file system wrapper.</param>
        /// <param name="restSharpFactory">The factory to create IRestClient instances.</param>
        /// <param name="logger">The logger to write to.</param>
        public OpenMeteo(IFileSystem fileSystem, IRestSharpClientFactory restSharpFactory, ILogger<OpenMeteo> logger)
        {
            _fileSystem = fileSystem;
            _restSharpFactory = restSharpFactory;
            _logger = logger;

            _lazyWmoCodesData = new(() => LoadWmoCodesData());
            _lazyDescriptions = new(() => BuildDescriptions());
            _lazyImageUrls = new(() => BuildImageUrls());
        }

        /// <inheritdoc/>
        public async Task<WeatherForecast> GetWeatherAsync(Coordinates coordinates, Guid correlationId, CancellationToken cancellationToken = default)
        {
            try
            {
                var resource = $"/v1/forecast?latitude={coordinates.Latitude}&longitude={coordinates.Longitude}&daily=weathercode,temperature_2m_max,temperature_2m_min,precipitation_probability_max&timeformat=unixtime&timezone=auto";
                var request = new RestRequest(resource);

                _logger.LogDebug("Creating RestClient for OpenMeteo. [{CorrelationId}]", correlationId);
                using var client = _restSharpFactory.CreateRestClient(new RestClientOptions(_baseUrl) { FailOnDeserializationError = true });

                _logger.LogDebug("Making GET request for weather. [{CorrelationId}]", correlationId);
                var response = await client.ExecuteGetAsync<WeatherResponse>(request, cancellationToken);
                if (response.IsSuccessful)
                    _logger.LogDebug("Successful GET request. Response {Status}.", response.StatusCode);
                else
                    _logger.LogWarning("Failed GET request. Response {Status}.", response.StatusCode);

                var daily = response.Data?.Daily;
                if (daily?.Time is not null)
                {
                    var items = new WeatherForecastItem[daily.Time.Length];
                    for (var i = 0; i < daily.Time.Length; i++)
                        items[i] = new(daily.Time[i], response.Data!.UtcOffsetSeconds, daily.WeatherCode[i], GetDescription(daily.WeatherCode[i]), GetImageUrl(daily.WeatherCode[i]), daily.Temperature2mMin[i], daily.Temperature2mMax[i], daily.PrecipitationProbabilityMax[i]);
                    return new(true, items, null);
                }

                if (response.Data?.Reason is not null)
                    _logger.LogWarning("OpenMeteo message: {Message}. [{CorrelationId}]", response.Data.Reason, correlationId);

                throw new WeatherException("No weather forecast obtained from OpenMeteo.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get weather at {Coordinates}. [{CorrelationId}]", coordinates, correlationId);
                if (ex is WeatherException)
                    throw;
                throw new WeatherException(ex.Message, ex);
            }
        }

        private Dictionary<int, Dictionary<string, Dictionary<string, string>>> LoadWmoCodesData()
        {
            // This uses the json file obtained from
            // https://gist.githubusercontent.com/stellasphere/9490c195ed2b53c707087c8c2db4ec0c/raw/76b0cb0ef0bfd8a2ec988aa54e30ecd1b483495d/descriptions.json
            try
            {
                const string jsonFile = "wmocodes.json";
                var jsonPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, jsonFile);
                if (_fileSystem.File.Exists(jsonPath))
                {
                    _logger.LogDebug("Found wmo codes file at {WMOFilePath}", jsonPath);
                    var json = _fileSystem.File.ReadAllText(jsonPath);
                    var dictionary = JsonSerializer.Deserialize<Dictionary<int, Dictionary<string, Dictionary<string, string>>>>(json);
                    return dictionary!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load WMO codes.");
            }
            return new();
        }

        private Dictionary<int, string> BuildDescriptions()
        {
            var data = _lazyWmoCodesData.Value;
            var dictionary = data.Keys.ToDictionary(_ => _, _ => data[_]["day"]["description"]);
            return dictionary;
        }

        private Dictionary<int, string> BuildImageUrls()
        {
            var data = _lazyWmoCodesData.Value;
            var dictionary = data.Keys.ToDictionary(_ => _, _ => data[_]["day"]["image"]);
            return dictionary;
        }

        private string GetDescription(int wmoCode) => _lazyDescriptions.Value.TryGetValue(wmoCode, out var description) ? description : "Clear";

        private string? GetImageUrl(int wmoCode) => _lazyImageUrls.Value.TryGetValue(wmoCode, out var url) ? url : null;
    }
}
