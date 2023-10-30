using CSharpFunctionalExtensions;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using System.Collections.Concurrent;
using Weather.Logic.Commands;
using Weather.Logic.Metrics;
using Weather.Logic.Queries;

namespace Weather.Logic.Tests.CommandHandlers.GenerateWeatherCommandHandler
{
    internal class GenerateWeatherCommandHandlerTestsContext
    {
        private readonly Fixture _fixture;
        private readonly MockQueue<WeatherCompleteEvent> _mockQueue;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IGenerateWeatherCommandHandlerMetrics> _mockMetrics;
        private readonly MockLogger<Weather.Logic.CommandHandlers.GenerateWeatherCommandHandler> _mockLogger;
        private readonly ConcurrentBag<Coordinates> _invalidCoordinates;
        private readonly ConcurrentBag<Coordinates> _exceptionCoordinates;
        private readonly ConcurrentDictionary<Coordinates, WeatherForecast> _weather;

        private string? _withExceptionMessage;
        private bool _validCoordinates;

        internal Weather.Logic.CommandHandlers.GenerateWeatherCommandHandler Sut { get; }

        public GenerateWeatherCommandHandlerTestsContext()
        {
            _fixture = new();
            _mockQueue = new();
            _mockMetrics = new();
            _mockLogger = new();
            _weather = new();
            _invalidCoordinates = new();
            _exceptionCoordinates = new();

            _withExceptionMessage = null;
            _validCoordinates = true;

            _mockMediator = new();
            _mockMediator.Setup(_ => _.Send(It.IsAny<GetWeatherQuery>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<Result<WeatherForecast>> query, CancellationToken _) => _weather[((GetWeatherQuery)query).Coordinates] = CreateWeatherForecast())
                .Returns((IRequest<Result<WeatherForecast>> query, CancellationToken _) => (_withExceptionMessage is not null) ? throw new InvalidOperationException(_withExceptionMessage) : GetWeather((GetWeatherQuery)query));

            Sut = new(_mockQueue.Object, _mockMediator.Object, _mockMetrics.Object, _mockLogger.Object);
        }

        private WeatherForecast CreateWeatherForecast() => new(true, Enumerable.Range(0, 7).Select(day => new WeatherForecastItem(DateTimeOffset.Now.AddDays(day).ToUnixTimeSeconds(), (int) DateTimeOffset.Now.Offset.TotalSeconds, _fixture.Create<int>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<double>(), _fixture.Create<double>(), _fixture.Create<int>())).ToArray(), null);

        private Task<Result<WeatherForecast>> GetWeather(GetWeatherQuery query) => Task.Run(() =>
        {
            var coordinates = query.Coordinates;
            return _exceptionCoordinates.Contains(query.Coordinates)
                ? throw new InvalidDataException(GetError(query))
                : _invalidCoordinates.Contains(query.Coordinates)
                    ? Result.Failure<WeatherForecast>(GetError(query))
                    : Result.Success(_weather[query.Coordinates]);
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
            _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
            return this;
        }

        internal GenerateWeatherCommandHandlerTestsContext AssertMetricsGuardTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal GenerateWeatherCommandHandlerTestsContext AssertMetricsWeatherTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordWeatherTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal GenerateWeatherCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Once);
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
                => (_.JobId == command.JobId)
                && (_.Weather.IsSuccessful == _validCoordinates)
                && (_.Weather.Error == (_validCoordinates ? null : GetError(command)))
                && (_.Weather.Items == (_validCoordinates ? _weather[command.Coordinates].Items : null)));
            Assert.That(published, Is.Not.Null);
            return this;
        }
    }
}
