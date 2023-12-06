using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace State.Application.Commands.NotifyJobStatusUpdate;

/// <summary>
/// Validation rules for <see cref="NotifyJobStatusUpdateCommand"/>.
/// </summary>
internal class NotifyJobStatusUpdateCommandValidator : AbstractValidator<NotifyJobStatusUpdateCommand>
{
    private readonly INotifyJobStatusUpdateCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyJobStatusUpdateCommandValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public NotifyJobStatusUpdateCommandValidator(INotifyJobStatusUpdateCommandHandlerMetrics metrics, ILogger<NotifyJobStatusUpdateCommandValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<NotifyJobStatusUpdateCommand> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}. [{CorrelationId}]", nameof(NotifyJobStatusUpdateCommand), result.ToString(), context.InstanceToValidate.JobId);
        return result;
    }
}
