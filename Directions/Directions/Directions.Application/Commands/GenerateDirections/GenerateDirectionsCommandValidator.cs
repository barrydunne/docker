using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Directions.Application.Commands.GenerateDirections;

/// <summary>
/// Validation rules for <see cref="GenerateDirectionsCommand"/>.
/// </summary>
internal class GenerateDirectionsCommandValidator : AbstractValidator<GenerateDirectionsCommand>
{
    private readonly IGenerateDirectionsCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateDirectionsCommandValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GenerateDirectionsCommandValidator(IGenerateDirectionsCommandHandlerMetrics metrics, ILogger<GenerateDirectionsCommandValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<GenerateDirectionsCommand> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}.", nameof(GenerateDirectionsCommand), result.ToString());
        return result;
    }
}
