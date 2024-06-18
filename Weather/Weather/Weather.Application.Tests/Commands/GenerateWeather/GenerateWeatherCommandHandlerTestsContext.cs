using AspNet.KickStarter.FunctionalResult;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using NSubstitute;
using System.Collections.Concurrent;
using Weather.Application.Commands.GenerateWeather;
using Weather.Application.Queries.GetWeather;

namespace Weather.Application.Tests.Commands.GenerateWeather;

internal class GenerateWeatherCommandHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly MockQueue<WeatherCompleteEvent> _mockQueue;
    private readonly ISender _mockMediator;
    private readonly IGenerateWeatherCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<GenerateWeatherCommandHandler> _mockLogger;
    private readonly ConcurrentBag<Coordinates> _invalidCoordinates;
    private readonly ConcurrentBag<Coordinates> _exceptionCoordinates;
    private readonly ConcurrentDictionary<Coordinates, WeatherForecast> _weather;

    private string? _withExceptionMessage;
    private bool _validCoordinates;

    internal GenerateWeatherCommandHandler Sut { get; }

    public GenerateWeatherCommandHandlerTestsContext()
    {
        _fixture = new();
        _mockQueue = new();
        _mockMetrics = Substitute.For<IGenerateWeatherCommandHandlerMetrics>();
        _mockLogger = new();
        _weather = new();
        _invalidCoordinates = new();
        _exceptionCoordinates = new();

        _withExceptionMessage = null;
        _validCoordinates = true;

        _mockMediator = Substitute.For<ISender>();
        _mockMediator
            .Send(Arg.Any<GetWeatherQuery>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => _withExceptionMessage is not null ? throw new InvalidOperationException(_withExceptionMessage) : GetWeather((GetWeatherQuery)callInfo.ArgAt<IRequest<Result<WeatherForecast>>>(0)))
            .AndDoes(callInfo => _weather[((GetWeatherQuery)callInfo.ArgAt<IRequest<Result<WeatherForecast>>>(0)).Coordinates] = CreateWeatherForecast());

        Sut = new(_mockQueue, _mockMediator, _mockMetrics, _mockLogger);
    }

    private WeatherForecast CreateWeatherForecast() => new(true, Enumerable.Range(0, 7).Select(day => new WeatherForecastItem(DateTimeOffset.Now.AddDays(day).ToUnixTimeSeconds(), (int)DateTimeOffset.Now.Offset.TotalSeconds, _fixture.Create<int>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>())).ToArray(), null);

    private Task<Result<WeatherForecast>> GetWeather(GetWeatherQuery query) => Task.Run(() =>
    {
        var coordinates = query.Coordinates;
        return _exceptionCoordinates.Contains(query.Coordinates)
            ? throw new InvalidDataException(GetError(query))
            : _invalidCoordinates.Contains(query.Coordinates)
                ? Result<WeatherForecast>.FromError(GetError(query))
                : Result<WeatherForecast>.Success(_weather[query.Coordinates]);
    });

    private string GetError(GetWeatherQuery query) => $"Invalid: {query.Coordinates.Latitude},{query.Coordinates.Longitude}";

    private string GetError(GenerateWeatherCommand command) => $"Invalid: {command.Coordinates.Latitude},{command.Coordinates.Longitude}";

    internal GenerateWeatherCommandHandlerTestsContext WithInvalidCoordinates(GenerateWeatherCommand command)
    {
        _validCoordinates = false;
        _invalidCoordinates.Add(command.Coordinates);
        return this;
    }

    internal GenerateWeatherCommandHandlerTestsContext WithCoordinatesException(GenerateWeatherCommand command)
    {
        _validCoordinates = false;
        _exceptionCoordinates.Add(command.Coordinates);
        return this;
    }

    internal GenerateWeatherCommandHandlerTestsContext WithException(string message)
    {
        _withExceptionMessage = message;
        return this;
    }

    internal GenerateWeatherCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal GenerateWeatherCommandHandlerTestsContext AssertMetricsWeatherTimeRecorded()
    {
        _mockMetrics.Received(1).RecordWeatherTime(Arg.Any<double>());
        return this;
    }

    internal GenerateWeatherCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Received(1).RecordPublishTime(Arg.Any<double>());
        return this;
    }

    internal GenerateWeatherCommandHandlerTestsContext AssertWeatherObtained(GenerateWeatherCommand command)
    {
        Assert.That(_weather.Keys, Does.Contain(command.Coordinates));
        return this;
    }

    internal GenerateWeatherCommandHandlerTestsContext AssertWeatherCompleteEventPublished(GenerateWeatherCommand command)
    {
        var published = _mockQueue.Messages.FirstOrDefault(_
            => _.JobId == command.JobId
            && _.Weather.IsSuccessful == _validCoordinates
            && _.Weather.Error == (_validCoordinates ? null : GetError(command))
            && _.Weather.Items == (_validCoordinates ? _weather[command.Coordinates].Items : null));
        Assert.That(published, Is.Not.Null);
        return this;
    }
}
