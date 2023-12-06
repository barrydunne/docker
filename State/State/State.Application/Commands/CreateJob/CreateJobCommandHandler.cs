using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using MediatR;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using State.Application.Commands.NotifyJobStatusUpdate;
using State.Application.Models;
using State.Application.Repositories;
using System.Diagnostics;

namespace State.Application.Commands.CreateJob;

/// <summary>
/// The handler for the <see cref="CreateJobCommand"/> command.
/// </summary>
internal class CreateJobCommandHandler : ICommandHandler<CreateJobCommand>
{
    private readonly IJobRepository _jobRepository;
    private readonly ISender _mediator;
    private readonly ICreateJobCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateJobCommandHandler"/> class.
    /// </summary>
    /// <param name="jobRepository">The repository for saving and retrieving jobs.</param>
    /// <param name="mediator">The mediator to send commands and queries to.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public CreateJobCommandHandler(IJobRepository jobRepository, ISender mediator, ICreateJobCommandHandlerMetrics metrics, ILogger<CreateJobCommandHandler> logger)
    {
        _jobRepository = jobRepository;
        _mediator = mediator;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(CreateJobCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(CreateJobCommandHandler), command.JobId);
        _metrics.IncrementCount();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Remove any existing job to always use the current data
            // This is to allow for a message being retried after a failure to
            // send the JobStatusUpdate event.
            await _jobRepository.DeleteJobByIdAsync(command.JobId, cancellationToken);

            // Create Job
            var job = new Job
            {
                JobId = command.JobId,
                StartingAddress = command.StartingAddress,
                DestinationAddress = command.DestinationAddress,
                Email = command.Email,
                CreatedUtc = DateTime.UtcNow
            };
            await _jobRepository.InsertAsync(job, cancellationToken);
            _metrics.RecordSaveTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            // Publish Event
            var result = await _mediator.Send(new NotifyJobStatusUpdateCommand(command.JobId, JobStatus.Processing, null));
            _metrics.RecordPublishTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create job. [{CorrelationId}]", command.JobId);
            return ex;
        }
    }
}
