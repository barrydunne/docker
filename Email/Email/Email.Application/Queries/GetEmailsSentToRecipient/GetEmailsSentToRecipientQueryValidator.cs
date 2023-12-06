using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Email.Application.Queries.GetEmailsSentToRecipient;

/// <summary>
/// Validation rules for <see cref="GetEmailsSentToRecipientQuery"/>.
/// </summary>
internal class GetEmailsSentToRecipientQueryValidator : AbstractValidator<GetEmailsSentToRecipientQuery>
{
    private readonly IGetEmailsSentToRecipientQueryHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmailsSentToRecipientQueryValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetEmailsSentToRecipientQueryValidator(IGetEmailsSentToRecipientQueryHandlerMetrics metrics, ILogger<GetEmailsSentToRecipientQueryValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.Email)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(_ => _.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(SearchRules.MaximumPageSize);

        RuleFor(_ => _.PageNumber)
            .GreaterThanOrEqualTo(1);
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<GetEmailsSentToRecipientQuery> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}.", nameof(GetEmailsSentToRecipientQuery), result.ToString());
        return result;
    }
}
