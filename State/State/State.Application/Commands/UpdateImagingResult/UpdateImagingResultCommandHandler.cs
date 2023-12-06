using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using MediatR;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using State.Application.Repositories;
using System.Diagnostics;

namespace State.Application.Commands.UpdateImagingResult;

/// <summary>
/// The handler for the <see cref="UpdateImagingResultCommand"/> command.
/// </summary>
internal class UpdateImagingResultCommandHandler : BaseUpdateResultCommandHandler, ICommandHandler<UpdateImagingResultCommand>
{
    private readonly IUpdateImagingResultCommandHandlerMetrics _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateImagingResultCommandHandler"/> class.
    /// </summary>
    /// <param name="jobRepository">The repository for saving and retrieving jobs.</param>
    /// <param name="mediator">The mediator to send commands and queries to.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public UpdateImagingResultCommandHandler(IJobRepository jobRepository, ISender mediator, IUpdateImagingResultCommandHandlerMetrics metrics, ILogger<UpdateImagingResultCommandHandler> logger) : base(jobRepository, mediator, logger)
        => _metrics = metrics;

    /// <inheritdoc/>
    public async Task<Result> Handle(UpdateImagingResultCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(UpdateImagingResultCommand), command.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _jobRepository.UpdateJobStatusAsync(command.JobId, command.Imaging.IsSuccessful, command.Imaging, cancellationToken);
            _metrics.RecordUpdateTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            // Check if the job is complete after any individual task completes
            var completed = await IsJobCompletedAsync(command.JobId, cancellationToken);
            if (completed)
                _metrics.RecordPublishTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update status. [{CorrelationId}]", command.JobId);
            return ex;
        }
    }
}
