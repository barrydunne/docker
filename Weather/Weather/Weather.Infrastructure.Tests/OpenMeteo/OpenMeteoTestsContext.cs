using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using RestSharp;
using System.Collections.Concurrent;
using System.IO.Abstractions.TestingHelpers;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using Weather.Infrastructure.ExternalApi.OpenMeteo;
using Weather.Infrastructure.ExternalApi.OpenMeteo.OpenMeteoModels;

namespace Weather.Infrastructure.Tests.OpenMeteo;

internal class OpenMeteoTestsContext
{
    private readonly Fixture _fixture;
    private readonly MockFileSystem _mockFileSystem;
    private readonly MockRestSharpFactory _mockRestSharpFactory;
    private readonly MockLogger<OpenMeteoApi> _mockLogger;
    private readonly ConcurrentDictionary<Coordinates, WeatherForecast> _knownWeather;

    private bool _withBadRequest;
    private bool _withNoResults;
    private string? _withExceptionMessage;

    internal OpenMeteoApi Sut { get; }

    public OpenMeteoTestsContext()
    {
        _fixture = new();
        _mockFileSystem = new();
        var jsonPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "wmocodes.json");
        var json = """
            {
                 "0": { "day": { "description": "Sunny", "image": "http://openweathermap.org/img/wn/01d@2x.png" } },
                 "1": { "day": { "description": "Mainly Sunny", "image": "http://openweathermap.org/img/wn/01d@2x.png" } },
                 "2": { "day": { "description": "Partly Cloudy", "image": "http://openweathermap.org/img/wn/02d@2x.png" } },
                 "3": { "day": { "description": "Cloudy", "image": "http://openweathermap.org/img/wn/03d@2x.png" } },
                "45": { "day": { "description": "Foggy", "image": "http://openweathermap.org/img/wn/50d@2x.png" } },
                "48": { "day": { "description": "Rime Fog", "image": "http://openweathermap.org/img/wn/50d@2x.png" } },
                "51": { "day": { "description": "Light Drizzle", "image": "http://openweathermap.org/img/wn/09d@2x.png" } }
            }
            """;
        _mockFileSystem.AddFile(jsonPath, new MockFileData(json));

        _mockRestSharpFactory = new();
        _mockRestSharpFactory.MockRestClient.ExecuteRequest = ExecuteRequest;

        _mockLogger = new();
        _knownWeather = new();
        _withNoResults = false;
        _withExceptionMessage = null;

        Sut = new(_mockFileSystem, _mockRestSharpFactory, _mockLogger);
    }

    private (HttpStatusCode StatusCode, string? Content, string? ContentType) ExecuteRequest(RestRequest request)
    {
        if (_withExceptionMessage is not null)
            throw new InvalidOperationException(_withExceptionMessage);
        if ((request.Method == Method.Get) && (request.Resource == "/v1/forecast"))
            return GetWeatherResponse(request);
        return MockRestClient.NotFoundResponse;
    }

    /// <summary>
    /// Replicate the types of responses returned by MapQuest in different conditions.
    /// </summary>
    /// <param name="request">The API request.</param>
    /// <returns>The status and content for the response.</returns>
    private (HttpStatusCode StatusCode, string? Content, string? ContentType) GetWeatherResponse(RestRequest request)
    {
        var latitudeParameter = request.Parameters.FirstOrDefault(_ => (_.Type == ParameterType.QueryString) && (_.Name == "latitude"))?.Value?.ToString() ?? string.Empty;
        var longitudeParameter = request.Parameters.FirstOrDefault(_ => (_.Type == ParameterType.QueryString) && (_.Name == "longitude"))?.Value?.ToString() ?? string.Empty;

        if (_withBadRequest || !(decimal.TryParse(latitudeParameter, out var latitude) && decimal.TryParse(longitudeParameter, out var longitude)))
            return (HttpStatusCode.BadRequest, """{ "error": true, "reason": "Latitude must be in range of -90 to 90°." }""", MediaTypeNames.Application.Json);
        if (_withNoResults)
            return (HttpStatusCode.OK, string.Empty, MediaTypeNames.Application.Json);

        var coordinates = new Coordinates(latitude, longitude);
        if (!_knownWeather.TryGetValue(coordinates, out var weather))
            weather = CreateWeatherForecast();

        var response = _fixture.Build<WeatherResponse>()
                               .With(_ => _.UtcOffsetSeconds, (int)DateTimeOffset.Now.Offset.TotalSeconds)
                               .Create();

        var items = weather!.Items!;
        response.Daily.Time = items.Select(_ => _.ForecastTimeUnixSeconds).ToArray();
        response.Daily.WeatherCode = items.Select(_ => _.WeatherCode).ToArray();
        response.Daily.Temperature2mMin = items.Select(_ => _.MinimumTemperatureC).ToArray();
        response.Daily.Temperature2mMax = items.Select(_ => _.MaximumTemperatureC).ToArray();
        response.Daily.PrecipitationProbabilityMax = items.Select(_ => _.PrecipitationProbabilityPercentage).ToArray();

        return (HttpStatusCode.OK, JsonSerializer.Serialize(response), MediaTypeNames.Application.Json);
    }

    private WeatherForecast CreateWeatherForecast() => new(true, Enumerable.Range(0, 7).Select(day => new WeatherForecastItem(DateTimeOffset.Now.AddDays(day).ToUnixTimeSeconds(), (int)DateTimeOffset.Now.Offset.TotalSeconds, _fixture.Create<int>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>())).ToArray(), null);

    internal OpenMeteoTestsContext WithWeather(Coordinates coordinates, WeatherForecast weather)
    {
        _knownWeather[coordinates] = weather;
        return this;
    }

    internal OpenMeteoTestsContext WithMissingJson()
    {
        foreach (var file in _mockFileSystem.AllFiles)
            _mockFileSystem.RemoveFile(file);
        return this;
    }

    internal OpenMeteoTestsContext WithInvalidJson()
    {
        foreach (var file in _mockFileSystem.AllFiles)
        {
            _mockFileSystem.RemoveFile(file);
            _mockFileSystem.AddFile(file, "INVALID");
        }
        return this;
    }

    internal OpenMeteoTestsContext WithBadRequest()
    {
        _withBadRequest = true;
        return this;
    }

    internal OpenMeteoTestsContext WithNoResult()
    {
        _withNoResults = true;
        return this;
    }

    internal OpenMeteoTestsContext WithException(string message)
    {
        _withExceptionMessage = message;
        return this;
    }
}
