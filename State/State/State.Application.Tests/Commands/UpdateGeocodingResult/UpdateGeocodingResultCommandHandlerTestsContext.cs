using AspNet.KickStarter.CQRS;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;
using Moq;
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
    private readonly Mock<ISender> _mockMediator;
    private readonly MockQueue<LocationsReadyEvent> _mockQueue;
    private readonly Mock<IUpdateGeocodingResultCommandHandlerMetrics> _mockMetrics;
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
        _mockMetrics = new();
        _mockLogger = new();
        _notifyJobStatusUpdateCommands = new();
        _notifyProcessingCompleteCommands = new();
        _invalidJobIds = new();
        _exceptionJobIds = new();

        _mockMediator = new();
        _mockMediator.Setup(_ => _.Send(It.IsAny<NotifyJobStatusUpdateCommand>(), It.IsAny<CancellationToken>()))
            .Callback((IRequest<Result> command, CancellationToken _) => _notifyJobStatusUpdateCommands.Add((NotifyJobStatusUpdateCommand)command))
            .Returns((IRequest<Result> command, CancellationToken _) => NotifyJobStatusUpdate((NotifyJobStatusUpdateCommand)command));

        Sut = new(_mockJobRepository.Object, _mockMediator.Object, _mockQueue.Object, _mockMetrics.Object, _mockLogger.Object);
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
        _mockQueue.Setup(_ => _.PublishAsync(It.IsAny<LocationsReadyEvent>(), It.IsAny<CancellationToken>())).Throws<InvalidOperationException>();
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext WithSendException(UpdateGeocodingResultCommand command)
    {
        _exceptionJobIds.Add(command.JobId);
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Verify(_ => _.IncrementCount(), Times.Once);
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext AssertMetricsUpdateTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordUpdateTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordPublishTime(It.IsAny<double>()), Times.Once);
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext AssertLocationsReadyEventPublished(UpdateGeocodingResultCommand command)
    {
        var published = _mockQueue.Messages.FirstOrDefault(_ => _.JobId == command.JobId);
        Assert.That(published, Is.Not.Null);
        return this;
    }

    internal UpdateGeocodingResultCommandHandlerTestsContext AssertNotifyProcessingCompleteCommandSent(Guid jobId)
    {
        Assert.That(_notifyJobStatusUpdateCommands, Has.Exactly(1).Matches<NotifyJobStatusUpdateCommand>(_ => _.JobId == jobId));
        return this;
    }
}
