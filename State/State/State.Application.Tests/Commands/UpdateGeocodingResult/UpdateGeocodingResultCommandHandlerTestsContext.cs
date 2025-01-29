using AspNet.KickStarter.FunctionalResult;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using State.Application.Commands.NotifyJobStatusUpdate;
using State.Application.Commands.NotifyProcessingComplete;
using State.Application.Commands.UpdateGeocodingResult;
using State.Application.Models;
using State.Application.Tests.Mocks;
using System.Collections.Concurrent;

namespace State.Application.Tests.Commands.UpdateGeocodingResult;

internal class UpdateGeocodingResultCommandHandlerTestsContext
{
    private readonly MockJobRepository _mockJobRepository;
    private readonly ISender _mockMediator;
    private readonly MockQueue<LocationsReadyEvent> _mockQueue;
    private readonly IUpdateGeocodingResultCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<UpdateGeocodingResultCommandHandler> _mockLogger;
    private readonly ConcurrentBag<NotifyJobStatusUpdateCommand> _notifyJobStatusUpdateCommands;
    private readonly ConcurrentBag<NotifyProcessingCompleteCommand> _notifyProcessingCompleteCommands;
    private readonly ConcurrentBag<Guid> _invalidJobIds;
    private readonly ConcurrentBag<Guid> _exceptionJobIds;

    internal UpdateGeocodingResultCommandHandler Sut { get; }

    public UpdateGeocodingResultCommandHandlerTestsContext()
    {
        _mockJobRepository = new();
        _mockQueue = new();
        _mockMetrics = Substitute.For<IUpdateGeocodingResultCommandHandlerMetrics>();
        _mockLogger = new();
        _notifyJobStatusUpdateCommands = new();
        _notifyProcessingCompleteCommands = new();
        _invalidJobIds = new();
        _exceptionJobIds = new();

        _mockMediator = Substitute.For<ISender>();
        _mockMediator
            .Send(Arg.Any<NotifyJobStatusUpdateCommand>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => NotifyJobStatusUpdate((NotifyJobStatusUpdateCommand)callInfo.ArgAt<IRequest<Result>>(0)))
            .AndDoes(callInfo => _notifyJobStatusUpdateCommands.Add((NotifyJobStatusUpdateCommand)callInfo.ArgAt<IRequest<Result>>(0)));

        Sut = new(_mockJobRepository, _mockMediator, _mockQueue, _mockMetrics, _mockLogger);
    }

    private Task<Result> NotifyJobStatusUpdate(NotifyJobStatusUpdateCommand command) => Task.Run(() =>
    {
        return _exceptionJobIds.Contains(command.JobId)
            ? throw new InvalidDataException(GetError(command))
            : _invalidJobIds.Contains(command.JobId)
                ? Result.FromError(GetError(command))
                : Result.Success();
    });

    internal string GetError(NotifyJobStatusUpdateCommand command) => $"Invalid: {command.JobId}";

    internal UpdateGeocodingResultCommandHandlerTestsContext WithJob(Job job)
    {
        _mockJobRepository.AddJob(job);
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext WithPublishException()
    {
        _mockQueue.WithPublishException();
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext WithSendException(UpdateGeocodingResultCommand command)
    {
        _exceptionJobIds.Add(command.JobId);
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext AssertMetricsUpdateTimeRecorded()
    {
        _mockMetrics.Received(1).RecordUpdateTime(Arg.Any<double>());
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Received(1).RecordPublishTime(Arg.Any<double>());
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext AssertLocationsReadyEventPublished(UpdateGeocodingResultCommand command)
    {
        var published = _mockQueue.Messages.FirstOrDefault(_ => _.JobId == command.JobId);
        published.ShouldNotBeNull();
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext AssertNotifyProcessingCompleteCommandSent(Guid jobId)
    {
        _notifyJobStatusUpdateCommands.Where(_ => _.JobId == jobId).ShouldHaveSingleItem();
        return this;
    }
}
