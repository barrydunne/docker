using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace State.Application.Commands.UpdateGeocodingResult;

/// <summary>
/// Validation rules for <see cref="UpdateGeocodingResultCommand"/>.
/// </summary>
internal class UpdateGeocodingResultCommandValidator : AbstractValidator<UpdateGeocodingResultCommand>
{
    private readonly IUpdateGeocodingResultCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateGeocodingResultCommandValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public UpdateGeocodingResultCommandValidator(IUpdateGeocodingResultCommandHandlerMetrics metrics, ILogger<UpdateGeocodingResultCommandValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(_ => _.StartingCoordinates)
            .NotNull();

        RuleFor(_ => _.DestinationCoordinates)
            .NotNull();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<UpdateGeocodingResultCommand> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}. [{CorrelationId}]", nameof(UpdateGeocodingResultCommand), result.ToString(), context.InstanceToValidate.JobId);
        return result;
    }
}
