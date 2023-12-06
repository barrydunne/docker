using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Geocoding.Application.Commands.GeocodeAddresses;

/// <summary>
/// Validation rules for <see cref="GeocodeAddressesCommand"/>.
/// </summary>
internal class GeocodeAddressesCommandValidator : AbstractValidator<GeocodeAddressesCommand>
{
    private readonly IGeocodeAddressesCommandHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeocodeAddressesCommandValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GeocodeAddressesCommandValidator(IGeocodeAddressesCommandHandlerMetrics metrics, ILogger<GeocodeAddressesCommandValidator> logger)
    {
        _metrics = metrics;
        _logger = logger;

        RuleFor(_ => _.JobId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

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
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<GeocodeAddressesCommand> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}.", nameof(GeocodeAddressesCommand), result.ToString());
        return result;
    }
}
