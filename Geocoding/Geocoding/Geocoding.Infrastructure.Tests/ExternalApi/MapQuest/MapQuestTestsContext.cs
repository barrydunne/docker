using Geocoding.Infrastructure.ExternalApi.MapQuest;
using Geocoding.Infrastructure.ExternalApi.MapQuest.MapQuestModels;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using RestSharp;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace Geocoding.Infrastructure.Tests.ExternalApi.MapQuest;

internal class MapQuestTestsContext
{
    private readonly Fixture _fixture;
    private readonly string _apiKey;
    private readonly MockCloudSecrets _mockCloudSecrets;
    private readonly MockRestSharpFactory _mockRestSharpFactory;
    private readonly MockLogger<MapQuestApi> _mockLogger;
    private readonly ConcurrentDictionary<string, Coordinates> _knownCoordinates;

    private string? _withExceptionMessage;

    internal MapQuestApi Sut { get; }

    public MapQuestTestsContext()
    {
        _fixture = new();
        _apiKey = _fixture.Create<string>();

        _mockCloudSecrets = new();
        _mockCloudSecrets.WithSecretValue("api.keys", "geocoding.mapquest", _apiKey);

        _mockRestSharpFactory = new();
        _mockRestSharpFactory.MockRestClient.ExecuteRequest = ExecuteRequest;

        _mockLogger = new();
        _knownCoordinates = new();
        _withExceptionMessage = null;

        Sut = new(_mockCloudSecrets.Object, _mockRestSharpFactory.Object, _mockLogger.Object);
    }

    private (HttpStatusCode StatusCode, string? Content, string? ContentType) ExecuteRequest(RestRequest request)
    {
        if (_withExceptionMessage is not null)
            throw new InvalidOperationException(_withExceptionMessage);
        if (request.Method == Method.Post && request.Resource == "/geocoding/v1/address")
            return GetGeocodingResponse(request);
        return MockRestClient.NotFoundResponse;
    }

    /// <summary>
    /// Replicate the types of responses returned by MapQuest in different conditions.
    /// </summary>
    /// <param name="request">The API request.</param>
    /// <returns>The status and content for the response.</returns>
    private (HttpStatusCode StatusCode, string? Content, string? ContentType) GetGeocodingResponse(RestRequest request)
    {
        if (request.Parameters.Where(_ => _.Type == ParameterType.QueryString).Select(_ => $"{_.Name}={_.Value}")?.FirstOrDefault() != $"key={_apiKey}")
            return (HttpStatusCode.Unauthorized, "The AppKey submitted with this request is invalid.", MediaTypeNames.Text.Plain);

        var bodyParameter = request.Parameters.FirstOrDefault(_ => _.Type == ParameterType.RequestBody) as BodyParameter;
        var geocodingRequest = bodyParameter?.Value as GeocodingRequest;
        var address = geocodingRequest?.Location;

        if (string.IsNullOrWhiteSpace(address))
        {
            var badResponse = new GeocodingResponse
            {
                Info = new()
                {
                    StatusCode = 400,
                    Messages = new[] { "Illegal argument from request: Insufficient info for location" }
                },
                Results = new[]
                {
                    new GeocodingResponseResult
                    {
                        ProvidedLocation = new(),
                        Locations = Array.Empty<GeocodingResponseLocation>()
                    }
                }
            };
            return (HttpStatusCode.OK, JsonSerializer.Serialize(badResponse), MediaTypeNames.Application.Json);
        }

        if (!_knownCoordinates.TryGetValue(address, out var coordinates))
            coordinates = _fixture.Create<Coordinates>();
        var location = _fixture.Build<GeocodingResponseLocation>()
                               .With(_ => _.LatLng, new GeocodingResponseLatLng { Lat = (double)coordinates.Latitude, Lng = (double)coordinates.Longitude })
                               .Create();
        // These are added for full branch coverage
        var missingLatLng = _fixture.Build<GeocodingResponseLocation>()
                               .Without(_ => _.LatLng)
                               .Create();
        var missingLat = _fixture.Build<GeocodingResponseLocation>()
                               .With(_ => _.LatLng, new GeocodingResponseLatLng { Lat = null, Lng = _fixture.Create<double>() })
                               .Create();
        var missingLng = _fixture.Build<GeocodingResponseLocation>()
                               .With(_ => _.LatLng, new GeocodingResponseLatLng { Lat = _fixture.Create<double>(), Lng = null })
                               .Create();
        var results = new GeocodingResponseResult[]
        {
            _fixture.Build<GeocodingResponseResult>()
                    .With(_ => _.Locations, new[] { missingLatLng, missingLat, missingLng, location })
                    .Create()
        };
        var response = _fixture.Build<GeocodingResponse>()
                               .With(_ => _.Results, results)
                               .Create();

        return (HttpStatusCode.OK, JsonSerializer.Serialize(response), MediaTypeNames.Application.Json);
    }

    internal MapQuestTestsContext WithAddressCoordinates(string address, Coordinates coordinates)
    {
        _knownCoordinates[address] = coordinates;
        return this;
    }

    internal MapQuestTestsContext WithSecretApiKey(string apiKey)
    {
        _mockCloudSecrets.WithSecretValue("api.keys", "geocoding.mapquest", apiKey);
        return this;
    }

    internal MapQuestTestsContext WithoutSecretApiKey()
    {
        _mockCloudSecrets.WithoutSecretValue("api.keys", "geocoding.mapquest");
        return this;
    }

    internal MapQuestTestsContext WithException(string message)
    {
        _withExceptionMessage = message;
        return this;
    }
}
