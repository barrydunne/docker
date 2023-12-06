using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Geocoding.Application.Queries.GetAddressCoordinates;

/// <summary>
/// Validation rules for <see cref="GetAddressCoordinatesQuery"/>.
/// </summary>
internal class GetAddressCoordinatesQueryValidator : AbstractValidator<GetAddressCoordinatesQuery>
{
    private readonly IGetAddressCoordinatesQueryHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAddressCoordinatesQueryValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetAddressCoordinatesQueryValidator(IGetAddressCoordinatesQueryHandlerMetrics metrics, ILogger<GetAddressCoordinatesQueryValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(_ => _.Address)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<GetAddressCoordinatesQuery> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}.", nameof(GetAddressCoordinatesQuery), result.ToString());
        return result;
    }
}
