using Email.Api.Models;
using Email.Application.Queries;
using FluentValidation;

namespace Email.Api.Validators;

/// <summary>
/// Validation rules for <see cref="GetEmailsSentBetweenTimesRequest"/>.
/// </summary>
public class GetEmailsSentBetweenTimesRequestValidator : AbstractValidator<GetEmailsSentBetweenTimesRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmailsSentBetweenTimesRequestValidator"/> class.
    /// </summary>
    public GetEmailsSentBetweenTimesRequestValidator()
    {
        RuleFor(_ => _.FromUnixSeconds)
            .GreaterThanOrEqualTo(SearchRules.MinimumTimeUnixSeconds)
            .LessThanOrEqualTo(SearchRules.MaximumTimeUnixSeconds);

        RuleFor(_ => _.ToUnixSeconds)
            .GreaterThanOrEqualTo(SearchRules.MinimumTimeUnixSeconds)
            .LessThanOrEqualTo(SearchRules.MaximumTimeUnixSeconds);

        RuleFor(_ => _.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(SearchRules.MaximumPageSize);

        RuleFor(_ => _.PageNumber)
            .GreaterThanOrEqualTo(1);
    }
}
