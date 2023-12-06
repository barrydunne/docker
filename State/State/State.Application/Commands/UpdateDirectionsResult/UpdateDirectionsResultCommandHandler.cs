using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using MediatR;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using State.Application.Repositories;
using System.Diagnostics;

namespace State.Application.Commands.UpdateDirectionsResult;

/// <summary>
/// The handler for the <see cref="UpdateDirectionsResultCommand"/> command.
/// </summary>
internal class UpdateDirectionsResultCommandHandler : BaseUpdateResultCommandHandler, ICommandHandler<UpdateDirectionsResultCommand>
{
    private readonly IUpdateDirectionsResultCommandHandlerMetrics _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDirectionsResultCommandHandler"/> class.
    /// </summary>
    /// <param name="jobRepository">The repository for saving and retrieving jobs.</param>
    /// <param name="mediator">The mediator to send commands and queries to.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public UpdateDirectionsResultCommandHandler(IJobRepository jobRepository, ISender mediator, IUpdateDirectionsResultCommandHandlerMetrics metrics, ILogger<UpdateDirectionsResultCommandHandler> logger) : base(jobRepository, mediator, logger)
        => _metrics = metrics;

    /// <inheritdoc/>
    public async Task<Result> Handle(UpdateDirectionsResultCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(UpdateDirectionsResultCommand), command.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _jobRepository.UpdateJobStatusAsync(command.JobId, command.Directions.IsSuccessful, command.Directions, cancellationToken);
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
