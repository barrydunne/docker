using Email.Api.Models;
using FluentValidation;

namespace Email.Api.Validators
{
    /// <summary>
    /// Validation rules for <see cref="GetEmailsSentBetweenTimesRequest"/>.
    /// </summary>
    public class GetEmailsSentBetweenTimesRequestValidator : AbstractValidator<GetEmailsSentBetweenTimesRequest>
    {
        private const long _minTime = 1672531200; // 2023-01-01 00:00:00
        private const long _maxTime = 4102444799; // 2099-12-31 23:59:59
        private const int _maxPageSize = 500;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmailsSentBetweenTimesRequestValidator"/> class.
        /// </summary>
        public GetEmailsSentBetweenTimesRequestValidator()
        {
            RuleFor(_ => _.FromUnixSeconds)
                .GreaterThanOrEqualTo(_minTime)
                .LessThanOrEqualTo(_maxTime);

            RuleFor(_ => _.ToUnixSeconds)
                .GreaterThanOrEqualTo(_minTime)
                .LessThanOrEqualTo(_maxTime);

            RuleFor(_ => _.PageSize)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(_maxPageSize);

            RuleFor(_ => _.PageNumber)
                .GreaterThanOrEqualTo(1);
        }
    }
}
