using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace PublicApi.Application.Commands.UpdateStatus;

/// <summary>
/// Validation rules for <see cref="UpdateStatusCommand"/>.
/// </summary>
internal class UpdateStatusCommandValidator : AbstractValidator<UpdateStatusCommand>
{
    private readonly IUpdateStatusCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateStatusCommandValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public UpdateStatusCommandValidator(IUpdateStatusCommandHandlerMetrics metrics, ILogger<UpdateStatusCommandValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<UpdateStatusCommand> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}. [{CorrelationId}]", nameof(UpdateStatusCommand), result.ToString(), context.InstanceToValidate.JobId);
        return result;
    }
}
