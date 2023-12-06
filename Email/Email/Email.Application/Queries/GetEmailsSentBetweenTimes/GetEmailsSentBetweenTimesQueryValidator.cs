using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Email.Application.Queries.GetEmailsSentBetweenTimes;

/// <summary>
/// Validation rules for <see cref="GetEmailsSentBetweenTimesQuery"/>.
/// </summary>
internal class GetEmailsSentBetweenTimesQueryValidator : AbstractValidator<GetEmailsSentBetweenTimesQuery>
{
    private readonly IGetEmailsSentBetweenTimesQueryHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmailsSentBetweenTimesQueryValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetEmailsSentBetweenTimesQueryValidator(IGetEmailsSentBetweenTimesQueryHandlerMetrics metrics, ILogger<GetEmailsSentBetweenTimesQueryValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.FromTime)
            .GreaterThanOrEqualTo(DateTimeOffset.FromUnixTimeSeconds(SearchRules.MinimumTimeUnixSeconds))
            .LessThanOrEqualTo(DateTimeOffset.FromUnixTimeSeconds(SearchRules.MaximumTimeUnixSeconds));

        RuleFor(_ => _.ToTime)
            .GreaterThanOrEqualTo(DateTimeOffset.FromUnixTimeSeconds(SearchRules.MinimumTimeUnixSeconds))
            .LessThanOrEqualTo(DateTimeOffset.FromUnixTimeSeconds(SearchRules.MaximumTimeUnixSeconds));

        RuleFor(_ => _.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(SearchRules.MaximumPageSize);

        RuleFor(_ => _.PageNumber)
            .GreaterThanOrEqualTo(1);
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<GetEmailsSentBetweenTimesQuery> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}.", nameof(GetEmailsSentBetweenTimesQuery), result.ToString());
        return result;
    }
}
