using FluentValidation;
using FluentValidation.Results;

namespace SecretsManager.Application.Commands.SetSecretValue;

/// <summary>
/// Validation rules for <see cref="SetSecretValueCommand"/>.
/// </summary>
internal class SetSecretValueCommandValidator : AbstractValidator<SetSecretValueCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SetSecretValueCommandValidator"/> class.
    /// </summary>
    public SetSecretValueCommandValidator()
    {
        RuleFor(_ => _.Vault)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(_ => _.Secret)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc/>
    public override Task<ValidationResult> ValidateAsync(ValidationContext<SetSecretValueCommand> context, CancellationToken cancellation = default)
    {
        return (context.InstanceToValidate == null)
        ? Task.FromResult(new ValidationResult([new ValidationFailure(nameof(SetSecretValueCommand), $"'{nameof(SetSecretValueCommand)}' must not be null.")]))
        : base.ValidateAsync(context, cancellation);
    }
}
