using FluentValidation;
using FluentValidation.Results;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Imaging.Application.Queries.GetImageUrl;

/// <summary>
/// Validation rules for <see cref="GetImageUrlQuery"/>.
/// </summary>
internal class GetImageUrlQueryValidator : AbstractValidator<GetImageUrlQuery>
{
    private readonly IGetImageUrlQueryHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetImageUrlQueryValidator"/> class.
    /// </summary>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetImageUrlQueryValidator(IGetImageUrlQueryHandlerMetrics metrics, ILogger<GetImageUrlQueryValidator> logger)
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

        RuleFor(_ => _.Coordinates)
            .Cascade(CascadeMode.Stop)
            .NotNull();
    }

    /// <inheritdoc/>
    public override async Task<ValidationResult> ValidateAsync(ValidationContext<GetImageUrlQuery> context, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.ValidateAsync(context, cancellation);
        _metrics.RecordGuardTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
        if (!result.IsValid)
            _logger.LogWarning("{Type} Validation failure: {Error}.", nameof(GetImageUrlQuery), result.ToString());
        return result;
    }
}
