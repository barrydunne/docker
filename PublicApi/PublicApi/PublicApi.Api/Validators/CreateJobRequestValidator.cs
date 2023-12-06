using FluentValidation;
using PublicApi.Api.Models;

namespace PublicApi.Api.Validators;

/// <summary>
/// Validation rules for <see cref="CreateJobRequest"/>.
/// </summary>
internal class CreateJobRequestValidator : AbstractValidator<CreateJobRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateJobRequestValidator"/> class.
    /// </summary>
    public CreateJobRequestValidator()
    {
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

        RuleFor(_ => _.Email)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(128);
    }
}
