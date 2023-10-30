using Ardalis.GuardClauses;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using CSharpFunctionalExtensions;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using PublicApi.Logic.Caching;
using PublicApi.Logic.Commands;
using PublicApi.Logic.Metrics;
using PublicApi.Repository;
using PublicApi.Repository.Models;
using System.Diagnostics;

namespace PublicApi.Logic.CommandHandlers
{
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

            var guardResult = PerformGuardChecks(command);
            _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
            if (guardResult.IsFailure)
                return Result.Failure(guardResult.Error);

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
                return Result.Failure(ex.Message);
            }
        }

        private Result PerformGuardChecks(UpdateStatusCommand command)
        {
            try
            {
                Guard.Against.NullOrEmpty(command.JobId, nameof(UpdateStatusCommand.JobId));
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to validate command properties. [{CorrelationId}]", command.JobId);
                return Result.Failure(ex.Message);
            }
        }
    }
}
