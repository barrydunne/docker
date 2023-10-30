using Email.Api.Models;
using FluentValidation;

namespace Email.Api.Validators
{
    /// <summary>
    /// Validation rules for <see cref="GetEmailsSentToRecipientRequest"/>.
    /// </summary>
    public class GetEmailsSentToRecipientRequestValidator : AbstractValidator<GetEmailsSentToRecipientRequest>
    {
        private const int _maxPageSize = 500;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmailsSentToRecipientRequestValidator"/> class.
        /// </summary>
        public GetEmailsSentToRecipientRequestValidator()
        {
            RuleFor(_ => _.RecipientEmail)
                .Cascade(CascadeMode.Stop) // If NotNull validator fails then do not run (and report) NotEmpty
                .NotNull()
                .NotEmpty()
                .EmailAddress();

            RuleFor(_ => _.PageSize)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(_maxPageSize);

            RuleFor(_ => _.PageNumber)
                .GreaterThanOrEqualTo(1);
        }
    }
}
