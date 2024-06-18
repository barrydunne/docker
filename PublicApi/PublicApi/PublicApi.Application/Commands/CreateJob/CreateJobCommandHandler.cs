using AspNet.KickStarter.CQRS.Abstractions.Commands;
using AspNet.KickStarter.FunctionalResult;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using PublicApi.Application.Models;
using PublicApi.Application.Repositories;
using System.Diagnostics;

namespace PublicApi.Application.Commands.CreateJob;

/// <summary>
/// The handler for the <see cref="CreateJobCommand"/> command.
/// </summary>
internal class CreateJobCommandHandler : ICommandHandler<CreateJobCommand, Guid>
{
    private readonly IQueue<JobCreatedEvent> _queue;
    private readonly IJobRepository _jobRepository;
    private readonly ICreateJobCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateJobCommandHandler"/> class.
    /// </summary>
    /// <param name="queue">The queue to publish the <see cref="JobCreatedEvent"/> to.</param>
    /// <param name="jobRepository">The repository for saving and retrieving jobs.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public CreateJobCommandHandler(IQueue<JobCreatedEvent> queue, IJobRepository jobRepository, ICreateJobCommandHandlerMetrics metrics, ILogger<CreateJobCommandHandler> logger)
    {
        _queue = queue;
        _jobRepository = jobRepository;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<Guid>> Handle(CreateJobCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("CreateJobCommand handler.");
        _metrics.IncrementCount();

        var jobId = Guid.NewGuid();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Note: Idempotency is handled by nginx, but perform check here for safety
            // Check idempotency key has not already been used by querying mongo on indexed property
            _logger.LogDebug("Checking idempotency key '{IdempotencyKey}' has not been used. [{CorrelationId}]", command.IdempotencyKey, jobId);
            var existingJobId = await _jobRepository.GetJobIdByIdempotencyKeyAsync(command.IdempotencyKey, cancellationToken);
            _metrics.RecordIdempotencyTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
            if (existingJobId is not null)
            {
                _logger.LogInformation("Found existing job. [{CorrelationId}]", jobId);
                return existingJobId.Value;
            }

            // Create the event here to use the same timestamp for the event and the db document
            var jobCreatedEvent = new JobCreatedEvent(jobId, command.StartingAddress, command.DestinationAddress, command.Email);

            // Save new document to db
            _logger.LogDebug("Creating new job document. [{CorrelationId}]", jobId);
            await _jobRepository.InsertAsync(new Job { JobId = jobId, IdempotencyKey = command.IdempotencyKey, Status = JobStatus.Accepted, CreatedUtc = jobCreatedEvent.CreatedUtc });
            _metrics.RecordSaveTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            // Publish new event to exchange
            _logger.LogDebug("Publishing job created event. [{CorrelationId}]", jobId);
            await _queue.PublishAsync(jobCreatedEvent, cancellationToken);
            _logger.LogInformation("Published job created event. [{CorrelationId}]", jobId);
            _metrics.RecordPublishTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            return jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create job. [{CorrelationId}]", jobId);
            return ex;
        }
    }
}
