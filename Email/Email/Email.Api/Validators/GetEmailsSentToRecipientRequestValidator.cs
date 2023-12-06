using Email.Api.Models;
using Email.Application.Queries;
using FluentValidation;

namespace Email.Api.Validators;

/// <summary>
/// Validation rules for <see cref="GetEmailsSentToRecipientRequest"/>.
/// </summary>
public class GetEmailsSentToRecipientRequestValidator : AbstractValidator<GetEmailsSentToRecipientRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmailsSentToRecipientRequestValidator"/> class.
    /// </summary>
    public GetEmailsSentToRecipientRequestValidator()
    {
        RuleFor(_ => _.RecipientEmail)
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
}
