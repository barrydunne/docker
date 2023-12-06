using FluentValidation;
using FluentValidation.Results;

namespace SecretsManager.Application.Queries.GetSecretValue;

/// <summary>
/// Validation rules for <see cref="GetSecretValueQuery"/>.
/// </summary>
internal class GetSecretValueQueryValidator : AbstractValidator<GetSecretValueQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetSecretValueQueryValidator"/> class.
    /// </summary>
    public GetSecretValueQueryValidator()
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
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetSecretValueQuery> context, CancellationToken cancellation = default)
    {
        return context.InstanceToValidate == null
        ? Task.FromResult(new ValidationResult([new ValidationFailure(nameof(GetSecretValueQuery), $"'{nameof(GetSecretValueQuery)}' must not be null.")]))
        : base.ValidateAsync(context, cancellation);
    }
}
