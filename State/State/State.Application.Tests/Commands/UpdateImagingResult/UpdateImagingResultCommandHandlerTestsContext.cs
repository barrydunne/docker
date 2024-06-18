using AspNet.KickStarter.FunctionalResult;
using MediatR;
using Microservices.Shared.Mocks;
using NSubstitute;
using State.Application.Commands.CreateJob;
using State.Application.Commands.NotifyJobStatusUpdate;
using State.Application.Commands.NotifyProcessingComplete;
using State.Application.Commands.UpdateImagingResult;
using State.Application.Models;
using State.Application.Tests.Mocks;
using System.Collections.Concurrent;

namespace State.Application.Tests.Commands.UpdateImagingResult;

internal class UpdateImagingResultCommandHandlerTestsContext
{
    private readonly MockJobRepository _mockJobRepository;
    private readonly ISender _mockMediator;
    private readonly IUpdateImagingResultCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<UpdateImagingResultCommandHandler> _mockLogger;
    private readonly ConcurrentBag<NotifyJobStatusUpdateCommand> _notifyJobStatusUpdateCommands;
    private readonly ConcurrentBag<NotifyProcessingCompleteCommand> _notifyProcessingCompleteCommands;
    private readonly ConcurrentBag<Guid> _invalidJobIds;
    private readonly ConcurrentBag<Guid> _exceptionJobIds;

    internal UpdateImagingResultCommandHandler Sut { get; }

    public UpdateImagingResultCommandHandlerTestsContext()
    {
        _mockJobRepository = new();
        _mockMetrics = Substitute.For<IUpdateImagingResultCommandHandlerMetrics>();
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
        _mockMediator
            .Send(Arg.Any<NotifyProcessingCompleteCommand>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Result.Success())
            .AndDoes(callInfo => _notifyProcessingCompleteCommands.Add((NotifyProcessingCompleteCommand)callInfo.ArgAt<IRequest<Result>>(0)));

        Sut = new(_mockJobRepository, _mockMediator, _mockMetrics, _mockLogger);
    }

    private Task<Result> NotifyJobStatusUpdate(NotifyJobStatusUpdateCommand command) => Task.Run(() =>
    {
        return _exceptionJobIds.Contains(command.JobId)
            ? throw new InvalidDataException(GetError(command))
            : _invalidJobIds.Contains(command.JobId)
                ? Result.FromError(GetError(command))
                : Result.Success();
    });

    internal string GetError(CreateJobCommand command) => $"Invalid: {command.JobId}";
    internal string GetError(NotifyJobStatusUpdateCommand command) => $"Invalid: {command.JobId}";

    internal Job? GetJob(Guid jobId) => _mockJobRepository.GetJob(jobId);

    internal UpdateImagingResultCommandHandlerTestsContext WithJob(Job job)
    {
        _mockJobRepository.AddJob(job);
        return this;
    }

    internal UpdateImagingResultCommandHandlerTestsContext WithJobRepositoryException()
    {
        _mockJobRepository.WithWriteException();
        return this;
    }

    internal UpdateImagingResultCommandHandlerTestsContext WithSendException(UpdateImagingResultCommand command)
    {
        _exceptionJobIds.Add(command.JobId);
        return this;
    }

    internal UpdateImagingResultCommandHandlerTestsContext WithSendFailure(UpdateImagingResultCommand command)
    {
        _invalidJobIds.Add(command.JobId);
        return this;
    }

    internal UpdateImagingResultCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal UpdateImagingResultCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Received(1).RecordPublishTime(Arg.Any<double>());
        return this;
    }

    internal UpdateImagingResultCommandHandlerTestsContext AssertMetricsPublishTimeNotRecorded()
    {
        _mockMetrics.Received(0).RecordPublishTime(Arg.Any<double>());
        return this;
    }

    internal UpdateImagingResultCommandHandlerTestsContext AssertNotifyJobStatusUpdateCommandSent(Guid jobId, bool error)
    {
        Assert.That(_notifyJobStatusUpdateCommands, Has.Exactly(1).Matches<NotifyJobStatusUpdateCommand>(_
            => _.JobId == jobId
            && string.IsNullOrWhiteSpace(_.Details) == !error));
        return this;
    }

    internal UpdateImagingResultCommandHandlerTestsContext AssertNotifyProcessingCompleteCommandSent(Guid jobId)
    {
        Assert.That(_notifyJobStatusUpdateCommands, Has.Exactly(1).Matches<NotifyJobStatusUpdateCommand>(_ => _.JobId == jobId));
        return this;
    }
}
