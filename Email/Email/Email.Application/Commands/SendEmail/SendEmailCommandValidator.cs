using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Email.Application.Commands.SendEmail;

/// <summary>
/// Validation rules for <see cref="SendEmailCommand"/>.
/// </summary>
internal class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
{
    private readonly ISendEmailCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendEmailCommandValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public SendEmailCommandValidator(ISendEmailCommandHandlerMetrics metrics, ILogger<SendEmailCommandValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(_ => _.Email)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(128);

        RuleFor(_ => _.StartingAddress)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(_ => _.DestinationAddress)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(_ => _.Directions)
            .Cascade(CascadeMode.Stop)
            .NotNull();

        RuleFor(_ => _.Weather)
            .Cascade(CascadeMode.Stop)
            .NotNull();

        RuleFor(_ => _.Imaging)
            .Cascade(CascadeMode.Stop)
            .NotNull();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<SendEmailCommand> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}.", nameof(SendEmailCommand), result.ToString());
        return result;
    }
}
