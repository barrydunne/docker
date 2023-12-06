using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace State.Application.Commands.UpdateDirectionsResult;

/// <summary>
/// Validation rules for <see cref="UpdateDirectionsResultCommand"/>.
/// </summary>
internal class UpdateDirectionsResultCommandValidator : AbstractValidator<UpdateDirectionsResultCommand>
{
    private readonly IUpdateDirectionsResultCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDirectionsResultCommandValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public UpdateDirectionsResultCommandValidator(IUpdateDirectionsResultCommandHandlerMetrics metrics, ILogger<UpdateDirectionsResultCommandValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(_ => _.Directions)
            .NotNull();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<UpdateDirectionsResultCommand> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}. [{CorrelationId}]", nameof(UpdateDirectionsResultCommand), result.ToString(), context.InstanceToValidate.JobId);
        return result;
    }
}
