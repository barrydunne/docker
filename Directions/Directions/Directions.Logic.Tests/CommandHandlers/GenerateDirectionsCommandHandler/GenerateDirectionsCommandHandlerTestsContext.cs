using CSharpFunctionalExtensions;
using Directions.Logic.Commands;
using Directions.Logic.Metrics;
using Directions.Logic.Queries;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
using System.Collections.Concurrent;

using EventDirections = Microservices.Shared.Events.Directions;

namespace Directions.Logic.Tests.CommandHandlers.GenerateDirectionsCommandHandler
{
    internal class GenerateDirectionsCommandHandlerTestsContext
    {
        private readonly Fixture _fixture;
        private readonly MockQueue<DirectionsCompleteEvent> _mockQueue;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IGenerateDirectionsCommandHandlerMetrics> _mockMetrics;
        private readonly MockLogger<Directions.Logic.CommandHandlers.GenerateDirectionsCommandHandler> _mockLogger;
        private readonly ConcurrentBag<string> _invalidCoordinates;
        private readonly ConcurrentBag<string> _exceptionCoordinates;
        private readonly ConcurrentDictionary<string, EventDirections> _directions;

        private string? _withExceptionMessage;
        private bool _validCoordinates;

        internal Directions.Logic.CommandHandlers.GenerateDirectionsCommandHandler Sut { get; }

        public GenerateDirectionsCommandHandlerTestsContext()
        {
            _fixture = new();
            _mockQueue = new();
            _mockMetrics = new();
            _mockLogger = new();
            _directions = new();
            _invalidCoordinates = new();
            _exceptionCoordinates = new();

            _withExceptionMessage = null;
            _validCoordinates = true;

            _mockMediator = new();
            _mockMediator.Setup(_ => _.Send(It.IsAny<GetDirectionsQuery>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<Result<EventDirections>> query, CancellationToken _) => _directions[GetKey((GetDirectionsQuery)query)] = _fixture.Build<EventDirections>().With(_ => _.IsSuccessful, true).With(_ => _.Error, (string?)null).Create())
                .Returns((IRequest<Result<EventDirections>> query, CancellationToken _) => (_withExceptionMessage is not null) ? throw new InvalidOperationException(_withExceptionMessage) : GetDirections((GetDirectionsQuery)query));

            Sut = new(_mockQueue.Object, _mockMediator.Object, _mockMetrics.Object, _mockLogger.Object);
        }

        private Task<Result<EventDirections>> GetDirections(GetDirectionsQuery query) => Task.Run(() =>
        {
            var startingCoordinates = query.StartingCoordinates;
            var destinationCoordinates = query.DestinationCoordinates;
            return _exceptionCoordinates.Contains(GetKey(query))
                ? throw new InvalidDataException(GetError(query))
                : _invalidCoordinates.Contains(GetKey(query))
                    ? Result.Failure<EventDirections>(GetError(query))
                    : Result.Success(_directions[GetKey(query)]);
        });

        private string GetKey(GetDirectionsQuery query) => $"{query.StartingCoordinates.Latitude},{query.StartingCoordinates.Longitude} to {query.DestinationCoordinates.Latitude},{query.DestinationCoordinates.Longitude}";

        private string GetKey(GenerateDirectionsCommand command) => GetKey(new GetDirectionsQuery(Guid.Empty, command.StartingCoordinates, command.DestinationCoordinates));

        private string GetError(GetDirectionsQuery query) => $"Invalid: {GetKey(query)}";

        private string GetError(GenerateDirectionsCommand command) => $"Invalid: {GetKey(command)}";

        internal GenerateDirectionsCommandHandlerTestsContext WithInvalidCoordinates(GenerateDirectionsCommand command)
        {
            _validCoordinates = false;
            _invalidCoordinates.Add(GetKey(command));
            return this;
        }

        internal GenerateDirectionsCommandHandlerTestsContext WithCoordinatesException(GenerateDirectionsCommand command)
        {
            _validCoordinates = false;
            _exceptionCoordinates.Add(GetKey(command));
            return this;
        }

        internal GenerateDirectionsCommandHandlerTestsContext WithException(string message)
        {
            _withExceptionMessage = message;
            return this;
        }

        internal GenerateDirectionsCommandHandlerTestsContext AssertMetricsCountIncremented()
        {
            _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
            return this;
        }

        internal GenerateDirectionsCommandHandlerTestsContext AssertMetricsGuardTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal GenerateDirectionsCommandHandlerTestsContext AssertMetricsDirectionsTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordDirectionsTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal GenerateDirectionsCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
        {
            _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Once);
            return this;
        }

        internal GenerateDirectionsCommandHandlerTestsContext AssertDirectionsObtained(GenerateDirectionsCommand command)
        {
            Assert.That(_directions.Keys, Does.Contain(GetKey(command)));
            return this;
        }

        internal GenerateDirectionsCommandHandlerTestsContext AssertDirectionsCompleteEventPublished(GenerateDirectionsCommand command)
        {
            var published = _mockQueue.Messages.FirstOrDefault(_
                => (_.JobId == command.JobId)
                && (_.Directions.IsSuccessful == _validCoordinates)
                && (_.Directions.Error == (_validCoordinates ? null : GetError(command)))
                && (_.Directions.Steps == (_validCoordinates ? _directions[GetKey(command)].Steps : null))
                && (_.Directions.TravelTimeSeconds == (_validCoordinates ? _directions[GetKey(command)].TravelTimeSeconds : null))
                && (_.Directions.DistanceKm == (_validCoordinates ? _directions[GetKey(command)].DistanceKm : null)));
            Assert.That(published, Is.Not.Null);
            return this;
        }
    }
}
