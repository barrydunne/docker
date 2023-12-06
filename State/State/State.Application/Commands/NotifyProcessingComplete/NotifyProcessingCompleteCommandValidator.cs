using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace State.Application.Commands.NotifyProcessingComplete;

/// <summary>
/// Validation rules for <see cref="NotifyProcessingCompleteCommand"/>.
/// </summary>
internal class NotifyProcessingCompleteCommandValidator : AbstractValidator<NotifyProcessingCompleteCommand>
{
    private readonly INotifyProcessingCompleteCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyProcessingCompleteCommandValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public NotifyProcessingCompleteCommandValidator(INotifyProcessingCompleteCommandHandlerMetrics metrics, ILogger<NotifyProcessingCompleteCommandValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<NotifyProcessingCompleteCommand> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}. [{CorrelationId}]", nameof(NotifyProcessingCompleteCommand), result.ToString(), context.InstanceToValidate.JobId);
        return result;
    }
}
