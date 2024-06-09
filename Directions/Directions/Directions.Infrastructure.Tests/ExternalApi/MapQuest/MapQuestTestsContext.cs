using Directions.Infrastructure.ExternalApi.MapQuest;
using Directions.Infrastructure.ExternalApi.MapQuest.MapQuestModels;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using RestSharp;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace Directions.Infrastructure.Tests.ExternalApi.MapQuest;

internal class MapQuestTestsContext
{
    private readonly Fixture _fixture;
    private readonly string _apiKey;
    private readonly MockCloudSecrets _mockCloudSecrets;
    private readonly MockRestSharpFactory _mockRestSharpFactory;
    private readonly MockRestSharpResiliencePipeline _mockRestSharpResiliencePipeline;
    private readonly MockLogger<MapQuestApi> _mockLogger;
    private readonly ConcurrentDictionary<string, Microservices.Shared.Events.Directions> _knownDirections;

    private bool _withNoResults;
    private bool _withNoLegs;
    private bool _withNoManeuvers;
    private bool _withNullNarrative;
    private string? _withExceptionMessage;

    internal MapQuestApi Sut { get; }

    public MapQuestTestsContext()
    {
        _fixture = new();
        _apiKey = _fixture.Create<string>();

        _mockCloudSecrets = new();
        _mockCloudSecrets.WithSecretValue("api.keys", "directions.mapquest", _apiKey);

        _mockRestSharpFactory = new();
        _mockRestSharpFactory.MockRestClient.ExecuteRequest = ExecuteRequest;

        _mockRestSharpResiliencePipeline = new();
        _mockLogger = new();
        _knownDirections = new();
        _withNoResults = false;
        _withNoLegs = false;
        _withNoManeuvers = false;
        _withExceptionMessage = null;

        Sut = new(_mockCloudSecrets, _mockRestSharpFactory, _mockRestSharpResiliencePipeline, _mockLogger);
    }

    private (HttpStatusCode StatusCode, string? Content, string? ContentType) ExecuteRequest(RestRequest request)
    {
        if (_withExceptionMessage is not null)
            throw new InvalidOperationException(_withExceptionMessage);
        if (request.Method == Method.Post && request.Resource == "/directions/v2/route")
            return GetDirectionsResponse(request);
        return MockRestClient.NotFoundResponse;
    }

    /// <summary>
    /// Replicate the types of responses returned by MapQuest in different conditions.
    /// </summary>
    /// <param name="request">The API request.</param>
    /// <returns>The status and content for the response.</returns>
    private (HttpStatusCode StatusCode, string? Content, string? ContentType) GetDirectionsResponse(RestRequest request)
    {
        if (request.Parameters.Where(_ => _.Type == ParameterType.QueryString).Select(_ => $"{_.Name}={_.Value}")?.FirstOrDefault() != $"key={_apiKey}")
            return (HttpStatusCode.Unauthorized, "The AppKey submitted with this request is invalid.", MediaTypeNames.Text.Plain);

        var bodyParameter = request.Parameters.FirstOrDefault(_ => _.Type == ParameterType.RequestBody) as BodyParameter;
        var directionsRequest = bodyParameter?.Value as DirectionsRequest;
        var locations = directionsRequest?.Locations;

        if ((locations?.Length ?? 0) != 2)
            return BadResponse(611, "At least two locations must be provided.");
        if (_withNoResults)
            return BadResponse(500, "Error processing request: Encountered an error while trying to batch geocode: Geocode Failed: 400: [\"Illegal argument from request: Invalid LatLng specified.[0]\"]");

        var startingCoordinates = GetCoordinates(locations![0]);
        var destinationCoordinates = GetCoordinates(locations[1]);

        if (!_knownDirections.TryGetValue(GetKey(startingCoordinates, destinationCoordinates), out var directions))
            directions = _fixture.Create<Microservices.Shared.Events.Directions>();

        var maneuvers = directions.Steps!.Select(_ => new DirectionsResponseManeuver
        {
            Distance = _.DistanceKm,
            Time = _.TravelTimeSeconds,
            Narrative = _.Description,
            StartPoint = _fixture.Create<DirectionsResponseStartPoint>()
        }).ToList();

        // Add a null narrative for testing
        if (_withNullNarrative)
            maneuvers.Add(new() { Distance = 0, Time = 0 });

        var leg = _fixture.Build<DirectionsResponseLeg>()
                          .With(_ => _.Maneuvers, _withNoManeuvers ? null : maneuvers)
                          .Create();

        var route = _fixture.Build<DirectionsResponseRoute>()
                    .With(_ => _.Distance, directions.Steps!.Sum(_ => _.DistanceKm))
                    .With(_ => _.RealTime, directions.Steps!.Sum(_ => _.TravelTimeSeconds))
                    .With(_ => _.Legs, _withNoLegs ? null : new List<DirectionsResponseLeg> { leg })
                    .Create();

        var response = _fixture.Build<DirectionsResponse>()
                               .With(_ => _.Route, route)
                               .Create();

        return (HttpStatusCode.OK, JsonSerializer.Serialize(response), MediaTypeNames.Application.Json);
    }

    private (HttpStatusCode StatusCode, string? Content, string? ContentType) BadResponse(int statusCode, string message)
    {
        var badResponse = new DirectionsResponse { Info = new() { StatusCode = statusCode, Messages = new[] { message } } };
        return (HttpStatusCode.OK, JsonSerializer.Serialize(badResponse), MediaTypeNames.Application.Json);
    }

    private static string GetKey(Coordinates startingCoordinates, Coordinates destinationCoordinates) => $"{startingCoordinates.Latitude},{startingCoordinates.Longitude} to {destinationCoordinates.Latitude},{destinationCoordinates.Longitude}";

    private Coordinates GetCoordinates(string location)
    {
        var parts = location.Split(',');
        return new Coordinates(decimal.Parse(parts[0]), decimal.Parse(parts[1]));
    }

    internal MapQuestTestsContext WithDirections(Coordinates startingCoordinates, Coordinates destinationCoordinates, Microservices.Shared.Events.Directions directions)
    {
        _knownDirections[GetKey(startingCoordinates, destinationCoordinates)] = directions;
        return this;
    }

    internal MapQuestTestsContext WithNoResult()
    {
        _withNoResults = true;
        return this;
    }

    internal MapQuestTestsContext WithNoLegs()
    {
        _withNoLegs = true;
        return this;
    }

    internal MapQuestTestsContext WithNoManeuvers()
    {
        _withNoManeuvers = true;
        return this;
    }

    internal MapQuestTestsContext WithNullNarrative()
    {
        _withNullNarrative = true;
        return this;
    }

    internal MapQuestTestsContext WithSecretApiKey(string apiKey)
    {
        _mockCloudSecrets.WithSecretValue("api.keys", "directions.mapquest", apiKey);
        return this;
    }

    internal MapQuestTestsContext WithoutSecretApiKey()
    {
        _mockCloudSecrets.WithoutSecretValue("api.keys", "directions.mapquest");
        return this;
    }

    internal MapQuestTestsContext WithException(string message)
    {
        _withExceptionMessage = message;
        return this;
    }
}
