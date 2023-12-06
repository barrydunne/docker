using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace State.Application.Commands.UpdateImagingResult;

/// <summary>
/// Validation rules for <see cref="UpdateImagingResultCommand"/>.
/// </summary>
internal class UpdateImagingResultCommandValidator : AbstractValidator<UpdateImagingResultCommand>
{
    private readonly IUpdateImagingResultCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateImagingResultCommandValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public UpdateImagingResultCommandValidator(IUpdateImagingResultCommandHandlerMetrics metrics, ILogger<UpdateImagingResultCommandValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(_ => _.Imaging)
            .NotNull();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<UpdateImagingResultCommand> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}. [{CorrelationId}]", nameof(UpdateImagingResultCommand), result.ToString(), context.InstanceToValidate.JobId);
        return result;
    }
}
