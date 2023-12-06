using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using PublicApi.Application.Caching;
using PublicApi.Application.Models;
using PublicApi.Application.Repositories;
using System.Diagnostics;

namespace PublicApi.Application.Commands.UpdateStatus;

/// <summary>
/// The handler for the <see cref="UpdateStatusCommand"/> command.
/// </summary>
internal class UpdateStatusCommandHandler : ICommandHandler<UpdateStatusCommand>
{
    private readonly IJobRepository _jobRepository;
    private readonly IJobCache _jobCache;
    private readonly IUpdateStatusCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatusCommandHandler"/> class.
    /// </summary>
    /// <param name="jobRepository">The repository for saving and retrieving jobs.</param>
    /// <param name="jobCache">The cache to save and retrieve jobs from in preference to using the repository.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public UpdateStatusCommandHandler(IJobRepository jobRepository, IJobCache jobCache, IUpdateStatusCommandHandlerMetrics metrics, ILogger<UpdateStatusCommandHandler> logger)
    {
        _jobRepository = jobRepository;
        _jobCache = jobCache;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(UpdateStatusCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(UpdateStatusCommand), command.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            _jobCache.Remove(command.JobId);

            // Update document in db
            _logger.LogDebug("Updating job status to {Status}. [{CorrelationId}]", command.Status, command.JobId);
            await _jobRepository.UpdateJobStatusAsync(command.JobId, command.Status, command.AdditionalInformation, cancellationToken);
            _metrics.RecordUpdateTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            // Only the JobId, Status and AdditionalInformation are required from the item in the cache so can store now
            _jobCache.Set(new Job { JobId = command.JobId, Status = command.Status, AdditionalInformation = command.AdditionalInformation }, TimeSpan.FromMinutes(10));

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update job. [{CorrelationId}]", command.JobId);
            return ex;
        }
    }
}
