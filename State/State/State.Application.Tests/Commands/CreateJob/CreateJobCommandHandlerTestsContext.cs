using AspNet.KickStarter.FunctionalResult;
using MediatR;
using Microservices.Shared.Mocks;
using State.Application.Commands.CreateJob;
using State.Application.Commands.NotifyJobStatusUpdate;
using State.Application.Tests.Mocks;
using System.Collections.Concurrent;

namespace State.Application.Tests.Commands.CreateJob;

internal class CreateJobCommandHandlerTestsContext
{
    private readonly MockJobRepository _mockJobRepository;
    private readonly ISender _mockMediator;
    private readonly ICreateJobCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<CreateJobCommandHandler> _mockLogger;
    private readonly ConcurrentBag<NotifyJobStatusUpdateCommand> _notifyJobStatusUpdateCommands;
    private readonly ConcurrentBag<Guid> _invalidJobIds;
    private readonly ConcurrentBag<Guid> _exceptionJobIds;

    internal CreateJobCommandHandler Sut { get; }

    public CreateJobCommandHandlerTestsContext()
    {
        _mockJobRepository = new();
        _mockMetrics = Substitute.For<ICreateJobCommandHandlerMetrics>();
        _mockLogger = new();
        _notifyJobStatusUpdateCommands = new();
        _invalidJobIds = new();
        _exceptionJobIds = new();

        _mockMediator = Substitute.For<ISender>();
        _mockMediator
            .Send(Arg.Any<NotifyJobStatusUpdateCommand>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => NotifyJobStatusUpdate((NotifyJobStatusUpdateCommand)callInfo.ArgAt<IRequest<Result>>(0)))
            .AndDoes(callInfo => _notifyJobStatusUpdateCommands.Add((NotifyJobStatusUpdateCommand)callInfo.ArgAt<IRequest<Result>>(0)));

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

    internal CreateJobCommandHandlerTestsContext WithSendException(CreateJobCommand command)
    {
        _exceptionJobIds.Add(command.JobId);
        return this;
    }

    internal CreateJobCommandHandlerTestsContext WithSendFailure(CreateJobCommand command)
    {
        _invalidJobIds.Add(command.JobId);
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertMetricsSaveTimeRecorded()
    {
        _mockMetrics.Received(1).RecordSaveTime(Arg.Any<double>());
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertMetricsPublishTimeRecorded()
    {
        _mockMetrics.Received(1).RecordPublishTime(Arg.Any<double>());
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertJobSaved(Guid jobId)
    {
        var job = _mockJobRepository.GetJob(jobId);
        job.ShouldNotBeNull();
        return this;
    }

    internal CreateJobCommandHandlerTestsContext AssertNotifyJobStatusUpdateCommandSent(Guid jobId)
    {
        _notifyJobStatusUpdateCommands.Where(_ => _.JobId == jobId).ShouldHaveSingleItem();
        return this;
    }
}
