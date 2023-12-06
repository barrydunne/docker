using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace State.Application.Commands.UpdateWeatherResult;

/// <summary>
/// Validation rules for <see cref="UpdateWeatherResultCommand"/>.
/// </summary>
internal class UpdateWeatherResultCommandValidator : AbstractValidator<UpdateWeatherResultCommand>
{
    private readonly IUpdateWeatherResultCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateWeatherResultCommandValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public UpdateWeatherResultCommandValidator(IUpdateWeatherResultCommandHandlerMetrics metrics, ILogger<UpdateWeatherResultCommandValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(_ => _.Weather)
            .NotNull();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<UpdateWeatherResultCommand> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}. [{CorrelationId}]", nameof(UpdateWeatherResultCommand), result.ToString(), context.InstanceToValidate.JobId);
        return result;
    }
}
