using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;

namespace Weather.Application.Queries.GetWeather;

/// <summary>
/// Validation rules for <see cref="GetWeatherQuery"/>.
/// </summary>
internal class GetWeatherQueryValidator : AbstractValidator<GetWeatherQuery>
{
    private readonly IGetWeatherQueryHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWeatherQueryValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetWeatherQueryValidator(IGetWeatherQueryHandlerMetrics metrics, ILogger<GetWeatherQueryValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(_ => _.Coordinates)
            .Cascade(CascadeMode.Stop)
            .NotNull();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<GetWeatherQuery> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}.", nameof(GetWeatherQuery), result.ToString());
        return result;
    }
}
