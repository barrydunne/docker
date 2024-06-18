using AspNet.KickStarter.FunctionalResult;
using Directions.Application.Commands.GenerateDirections;
using Directions.Application.Queries.GetDirections;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using NSubstitute;
using System.Collections.Concurrent;

using EventDirections = Microservices.Shared.Events.Directions;

namespace Directions.Application.Tests.CommandHandlers.GenerateDirectionsCommandHandler;

internal class GenerateDirectionsCommandHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly MockQueue<DirectionsCompleteEvent> _mockQueue;
    private readonly ISender _mockMediator;
    private readonly IGenerateDirectionsCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<Commands.GenerateDirections.GenerateDirectionsCommandHandler> _mockLogger;
    private readonly ConcurrentBag<string> _invalidCoordinates;
    private readonly ConcurrentBag<string> _exceptionCoordinates;
    private readonly ConcurrentDictionary<string, EventDirections> _directions;

    private string? _withExceptionMessage;
    private bool _validCoordinates;

    internal Commands.GenerateDirections.GenerateDirectionsCommandHandler Sut { get; }

    public GenerateDirectionsCommandHandlerTestsContext()
    {
        _fixture = new();
        _mockQueue = new();
        _mockMetrics = Substitute.For<IGenerateDirectionsCommandHandlerMetrics>();
        _mockLogger = new();
        _directions = new();
        _invalidCoordinates = new();
        _exceptionCoordinates = new();

        _withExceptionMessage = null;
        _validCoordinates = true;

        _mockMediator = Substitute.For<ISender>();
        _mockMediator
            .Send(Arg.Any<GetDirectionsQuery>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => (_withExceptionMessage is not null) ? throw new InvalidOperationException(_withExceptionMessage) : GetDirections((GetDirectionsQuery)callInfo.ArgAt<IRequest<Result<EventDirections>>>(0)))
            .AndDoes(callInfo => _directions[GetKey((GetDirectionsQuery)callInfo.ArgAt<IRequest<Result<EventDirections>>>(0))] = _fixture.Build<EventDirections>().With(_ => _.IsSuccessful, true).With(_ => _.Error, (string?)null).Create());

        Sut = new(_mockQueue, _mockMediator, _mockMetrics, _mockLogger);
    }

    private Task<Result<EventDirections>> GetDirections(GetDirectionsQuery query) => Task.Run(() =>
    {
        var startingCoordinates = query.StartingCoordinates;
        var destinationCoordinates = query.DestinationCoordinates;
        return _exceptionCoordinates.Contains(GetKey(query))
            ? throw new InvalidDataException(GetError(query))
            : _invalidCoordinates.Contains(GetKey(query))
                ? Result<EventDirections>.FromError(GetError(query))
                : Result<EventDirections>.Success(_directions[GetKey(query)]);
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
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal GenerateDirectionsCommandHandlerTestsContext AssertMetricsDirectionsTimeRecorded()
    {
        _mockMetrics.Received(1).RecordDirectionsTime(Arg.Any<double>());
        return this;
    }

    internal GenerateDirectionsCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Received(1).RecordPublishTime(Arg.Any<double>());
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
